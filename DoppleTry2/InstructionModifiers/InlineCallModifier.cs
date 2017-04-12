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
using Dopple.VerifierNs;

namespace Dopple.InstructionModifiers
{
    class CallInliner
    {
        readonly ProgramFlowManager programFlowHanlder = new ProgramFlowManager();
        private readonly Dictionary<MethodDefinition, int> inlinedInstancesCountPerMethod = new Dictionary<MethodDefinition, int>();
        private InstructionNodeFactory _InstructionNodeFactory;
        private TraceManager _BackTraceManager = new TraceManager();
        
        //TODO remove
        public Verifier[] Verifiers { get; set; }
        public CallInliner(InstructionNodeFactory instructionNodeFactory)
        {
            _InstructionNodeFactory = instructionNodeFactory;
        }
        //problem, need to mark the originals if the son is discovered to be reucrisve
        public void InlineCallNodes(List<InstructionNode> instructionNodes)
        {
            List<InstructionNode> originalNodes = new List<InstructionNode>(instructionNodes);
            instructionNodes.ForEach(x => x.InliningProperties.CallSequence.Add(new MethodAndNode() { Method = instructionNodes[0].Method, MethodsNodes = originalNodes }));
            var callNodes = instructionNodes.Where(x => x is InlineableCallNode).Cast<InlineableCallNode>().Where(x => !x.CallWasInlined).ToArray();
            foreach (var callNode in callNodes)
            {
                instructionNodes.InsertRange(instructionNodes.IndexOf(callNode)+1, InlineRec(callNode));
               
            }
            foreach (var inlinedCallNode in instructionNodes.Where(x => x is InlineableCallNode && ((InlineableCallNode) x).CallWasInlined).ToList())
            {
                inlinedCallNode.SelfRemove();
                instructionNodes.Remove(inlinedCallNode);
            }
        }

        private List<InstructionNode> InlineRec(InlineableCallNode callNode)
        {
            //var tempStop = Stopwatch.StartNew();
            //if (callNode.InliningProperties.CallSequence.Count > 8)
            //{
            //    //return new List<InstructionNode>();
            //}
            MethodDefinition calledMethodDef = callNode.TargetMethodDefinition;
            callNode.CallWasInlined = true;
            if (calledMethodDef.Body == null)
            {
                return new List<InstructionNode>();
            }
            var isSecondLevelRecursiveCall = callNode.InliningProperties.CallSequence.Count(x => x.Method == callNode.TargetMethod) > 1;
            if (isSecondLevelRecursiveCall)
            {
                return new List<InstructionNode>();
            }
            callNode.StackPushCount = 0;
            List<InstructionNode> callNodeOriginalForwardRoutes = callNode.ProgramFlowForwardRoutes.ToList();

            List<InstructionNode> inlinedNodes = calledMethodDef.Body.Instructions.SelectMany(x => _InstructionNodeFactory.GetInstructionNodes(x, calledMethodDef)).ToList();
          
            inlinedNodes.ForEach(x => SetNodeProps(x, inlinedNodes, callNode));
            callNode.BranchProperties.Branches.ForEach(x => x.BranchNodes.InsertRange(x.BranchNodes.IndexOf(callNode)+1, inlinedNodes));

            programFlowHanlder.AddFlowConnections(inlinedNodes);
            _BackTraceManager.DataTraceInFunctionBounds(inlinedNodes);

            StitchProgramFlow(callNode, inlinedNodes[0]);

            var retNodes = inlinedNodes.Where(x => x is RetInstructionNode).ToArray();
            foreach (var forwardNode in callNode.DataFlowForwardRelated)
            {
                forwardNode.MirrorArg.ContainingList.AddTwoWay(retNodes, forwardNode.MirrorArg.ArgIndex);
            }

            foreach (var lastInlinedNode in inlinedNodes.Where(x => x.ProgramFlowForwardRoutes.Count == 0))
            {
                StitchProgramFlow(lastInlinedNode, callNodeOriginalForwardRoutes);
            }
            foreach (InlineableCallNode secondLevelInlinedCallNode in inlinedNodes.Where(x => x is InlineableCallNode).ToList())
            {
                inlinedNodes.InsertRange(inlinedNodes.IndexOf(secondLevelInlinedCallNode)+1, InlineRec(secondLevelInlinedCallNode));
            }
            if (!inlinedNodes.Where(x => x is InlineableCallNode).Any())
            {
             //   Console.WriteLine(callNode.TargetMethod.FullName + " is the last of the chain");
            }
            //tempStop.Stop();
            //Console.WriteLine("inlinling " + callNode.TargetMethod.FullName + " with " + inlinedNodes.Count + " nodes, took " + tempStop.Elapsed);
            return inlinedNodes;
        }

        public void RemoveCallNodes(List<InstructionNode> instructionNodes)
        {
            foreach (var inlinedCallNode in instructionNodes.Where(x => x is InlineableCallNode && !(x is ConstructorCallNode)).ToArray())
            {
                inlinedCallNode.SelfRemove();
                instructionNodes.Remove(inlinedCallNode);
            }
        }
        public void MergeRecursiveNodes(List<InstructionNode> instructionNodes)
        {            
            foreach (var recusriveNode in instructionNodes.Where(x => x.InliningProperties.Recursive).ToList())
            {
                MethodAndNode firstCallNodes = recusriveNode.InliningProperties.CallSequence.Where(x => x.Method == recusriveNode.Method).First();
                var equivilentNodes = firstCallNodes.MethodsNodes.Where(x => x.Instruction.Offset == recusriveNode.Instruction.Offset);
                if (equivilentNodes.Count() != 1)
                {
                    throw new Exception("Only one matching node should exist");
                }
                var equivilentNode = equivilentNodes.First();
                bool equivilentWasRemoved = !instructionNodes.Contains(equivilentNode);
                if (equivilentWasRemoved)
                {
                    recusriveNode.SelfRemove();
                }
                else
                {
                    recusriveNode.MergeInto(equivilentNodes.First(), false);
                }
                instructionNodes.Remove(recusriveNode);
                instructionNodes.ForEach(x => Verifiers.ForEach(y => y.Verify(x)));
            }
        }

        private void SetNodeProps(InstructionNode inlinedNode, List<InstructionNode> nodes, InlineableCallNode callNode)
        {
            inlinedNode.InliningProperties.Inlined = true;
            inlinedNode.InliningProperties.CallNode = callNode;
            inlinedNode.InliningProperties.CallSequence = callNode.InliningProperties.CallSequence.ToList();
            inlinedNode.BranchProperties.Branches.AddRangeDistinct(callNode.BranchProperties.Branches);
            inlinedNode.InliningProperties.CallSequence.Add(new MethodAndNode() { Method = callNode.TargetMethodDefinition, MethodsNodes = new List<InstructionNode>(nodes) });
            SetRecursionIndex(inlinedNode, nodes);
        }

        private void SetRecursionIndex(InstructionNode inlinedNode, List<InstructionNode> higherLevelNodes)
        {
            if (inlinedNode.InliningProperties.CallSequence.Count(x => x.Method == inlinedNode.Method) >1)
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
