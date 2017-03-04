using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Dopple.BackTracers;
using Dopple.ProgramFlowHanlder;
using Dopple.InstructionNodes;
using System.Diagnostics;

namespace Dopple.InstructionModifiers
{
    class InlineCallModifier : IPreBacktraceModifier
    {
        readonly ProgramFlowManager programFlowHanlder = new ProgramFlowManager();
        private readonly Dictionary<MethodDefinition, int> inlinedInstancesCountPerMethod = new Dictionary<MethodDefinition, int>();
        private InstructionNodeFactory _InstructionNodeFactory;
        private BackTraceManager _BackTraceManager = new BackTraceManager();
        
        public InlineCallModifier(InstructionNodeFactory instructionNodeFactory)
        {
            _InstructionNodeFactory = instructionNodeFactory;
        }
        //problem, need to mark the originals if the son is discovered to be reucrisve
        public void Modify(List<InstructionNode> instructionNodes)
        {
            List<InstructionNode> originalNodes = new List<InstructionNode>(instructionNodes);
            var callNodes = instructionNodes.Where(x => x is InlineableCallNode).Cast<InlineableCallNode>().ToArray();
            var startCallSequence = new List<MethodDefinition>() { instructionNodes[0].Method };
            foreach (var callNode in callNodes)
            {
                instructionNodes.InsertRange(instructionNodes.IndexOf(callNode), InlineRec(callNode, instructionNodes));
            }
            bool topMostMethodIsRecursive = instructionNodes.Any(x => x is InlineableCallNode && ((InlineableCallNode) x).TargetMethod == originalNodes[0].Method);
            if (topMostMethodIsRecursive)
            {
                foreach(var originalNode in originalNodes)
                {
                    originalNode.InliningProperties.Recursive = true;
                    originalNode.InliningProperties.RecursionInstanceIndex = -1;
                }
            }
        }

        private List<InstructionNode> InlineRec(InlineableCallNode inlinedCallNode, List<InstructionNode> originalNodes)
        {
            List<InstructionNode> callNodeOriginalForwardRoutes = inlinedCallNode.ProgramFlowForwardRoutes.ToList();
            MethodDefinition calledMethodDef = inlinedCallNode.TargetMethodDefinition;
            List<InstructionNode> originalNodesClone = new List<InstructionNode>(originalNodes);

            if (calledMethodDef.Body == null)
            {  
                return new List<InstructionNode>();
            }

            var isSecondLevelRecursiveCall = inlinedCallNode.InliningProperties.CallSequence.Count(x => x.Method == inlinedCallNode.TargetMethod) > 1;
            if (isSecondLevelRecursiveCall)
            {
                return new List<InstructionNode>();
            }
            inlinedCallNode.StackPushCount = 0;
            List<InstructionNode> inlinedNodes = calledMethodDef.Body.Instructions.SelectMany(x => _InstructionNodeFactory.GetInstructionNodes(x, calledMethodDef)).ToList();
            inlinedNodes.ForEach(x => SetNodeProps(x, callSequenceClone, inlinedCallNode));
            programFlowHanlder.AddFlowConnections(inlinedNodes);
            _BackTraceManager.BackTraceInFunctionBounds(inlinedNodes);
            StitchProgramFlow(inlinedCallNode, inlinedNodes[0]);
            foreach (var lastInlinedNode in inlinedNodes.Where(x => x.ProgramFlowForwardRoutes.Count == 0))
            {
                StitchProgramFlow(lastInlinedNode, callNodeOriginalForwardRoutes);
            }
            callSequenceClone.Add(calledMethodDef);
            foreach (InlineableCallNode secondLevelInlinedCallNode in inlinedNodes.Where(x => x is InlineableCallNode).ToList())
            {
                inlinedNodes.InsertRange(inlinedNodes.IndexOf(secondLevelInlinedCallNode), InlineRec(secondLevelInlinedCallNode, callSequenceClone));
            }
            return inlinedNodes;
        }

        private void SetNodeProps(InstructionNode inlinedNode, List<MethodDefinition> callSequence, InlineableCallNode callNode)
        {
            inlinedNode.InliningProperties.Inlined = true;
            inlinedNode.InliningProperties.CallNode = callNode;
            SetRecursionRelatedProps(inlinedNode, callSequence);
            inlinedNode.ProgramFlowBackAffected.AddTwoWay(callNode.ProgramFlowBackAffected);
            inlinedNode.InliningProperties.CallSequence = callSequence;
        }

        private void SetRecursionRelatedProps(InstructionNode inlinedNode, List<MethodDefinition> callSequence)
        {
            if (callSequence.Contains(inlinedNode.Method))
            {
                inlinedNode.InliningProperties.Recursive = true;
            }
            if (!inlinedInstancesCountPerMethod.ContainsKey(inlinedNode.Method))
            {
                inlinedInstancesCountPerMethod.Add(inlinedNode.Method, 0);
            }
            inlinedNode.InliningProperties.RecursionInstanceIndex = inlinedInstancesCountPerMethod[inlinedNode.Method];
            inlinedInstancesCountPerMethod[inlinedNode.Method]++;
        }

        private static void StitchProgramFlow(InstructionNode backNode, InstructionNode forwardNode)
        {
            StitchProgramFlow(backNode, new InstructionNode[] { forwardNode });
        }

        private static void StitchProgramFlow(InstructionNode backNode, IEnumerable<InstructionNode> forwardNodes)
        {
            foreach (var forwardFlowInst in backNode.ProgramFlowForwardRoutes.ToList())
            {
                forwardFlowInst.ProgramFlowBackRoutes.RemoveTwoWay(backNode);
            }
            foreach (var newForwardNode in forwardNodes)
            {
                newForwardNode.ProgramFlowBackRoutes.AddTwoWay(backNode);
            }
        }
    }
}
