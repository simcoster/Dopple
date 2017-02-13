using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using DoppleTry2.BackTrackers;
using DoppleTry2.ProgramFlowHanlder;
using DoppleTry2.InstructionNodes;
using System.Diagnostics;

namespace DoppleTry2.InstructionModifiers
{
    class InlineCallModifier : IPreBacktraceModifier
    {
        readonly ProgramFlowManager programFlowHanlder = new ProgramFlowManager();
        private readonly Dictionary<MethodDefinition, int> inlinedInstancesCountPerMethod = new Dictionary<MethodDefinition, int>();
        private InstructionNodeFactory _InstructionNodeFactory = new InstructionNodeFactory();

        public void Modify(List<InstructionNode> instructionNodes)
        {
            var callNodes = instructionNodes.Where(x => x is InlineableCallNode).Cast<InlineableCallNode>().ToArray();
            var startCallSequence = new List<MethodDefinition>() { instructionNodes[0].Method };
            foreach (var callNode in callNodes)
            {
                instructionNodes.InsertRange(instructionNodes.IndexOf(callNode), InlineRec(callNode, startCallSequence));
            }
        }

        private List<InstructionNode> InlineRec(InlineableCallNode callNode, List<MethodDefinition> callSequence)
        {
            List<InstructionNode> callNodeOriginalForwardRoutes = callNode.ProgramFlowForwardRoutes.ToList();
            MethodDefinition calledFunc = callNode.CalledFunction;
            var callSequenceClone = new List<MethodDefinition>(callSequence);
            if (calledFunc.Body == null)
            {  
                return new List<InstructionNode>();
            }
            if (callSequenceClone.Count(x => x == callNode.Method) > 1)
            {
                return new List<InstructionNode>();
            }
            else
            {
                callNode.StackPushCount = 0;
            }
            List<InstructionNode> inlinedNodes = calledFunc.Body.Instructions.Select(x => _InstructionNodeFactory.GetInstructionWrapper(x, calledFunc)).ToList();
            inlinedNodes.ForEach(x => SetCallNodeProps(x, callSequenceClone));
            programFlowHanlder.AddFlowConnections(inlinedNodes);
            StitchProgramFlow(callNode, inlinedNodes[0]);
            foreach (var lastInlinedNode in inlinedNodes.Where(x => x.ProgramFlowForwardRoutes.Count == 0))
            {
                StitchProgramFlow(lastInlinedNode, callNodeOriginalForwardRoutes);
            }
            callSequenceClone.Add(calledFunc);
            foreach (InlineableCallNode inlinedCallNode in inlinedNodes.Where(x => x is InlineableCallNode).ToList())
            {
                inlinedNodes.InsertRange(inlinedNodes.IndexOf(inlinedCallNode), InlineRec(inlinedCallNode, callSequenceClone));
            }
            return inlinedNodes;
        }

        private void SetCallNodeProps(InstructionNode inlinedNode, List<MethodDefinition> callSequence)
        {
            inlinedNode.InliningProperties.Inlined = true;
            if (callSequence.Contains(inlinedNode.Method))
            {
                inlinedNode.InliningProperties.Recursive = true;
                inlinedNode.InliningProperties.RecursionLevel = callSequence.Count(x => x == inlinedNode.Method);
            }
            else
            {
                inlinedNode.InliningProperties.Recursive = false;
            }
            if (!inlinedInstancesCountPerMethod.ContainsKey(inlinedNode.Method))
            {
                inlinedInstancesCountPerMethod.Add(inlinedNode.Method, 0);
            }
            inlinedNode.InliningProperties.SameMethodCallIndex = inlinedInstancesCountPerMethod[inlinedNode.Method];
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
