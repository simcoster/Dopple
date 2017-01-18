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
        public static readonly Code[] CallOpCodes = CodeGroups.CallCodes;
        readonly ProgramFlowManager programFlowHanlder = new ProgramFlowManager();
        private static List<InstructionNode> MyInstructionNodes;
        private readonly Dictionary<MethodDefinition, int> inlinedCountPerMethod = new Dictionary<MethodDefinition, int>();

        public void Modify(List<InstructionNode> instructionNodes)
        {
            MyInstructionNodes = instructionNodes;
            var callNodes = instructionNodes.Where(x => x is InlineableCallNode).Cast<InlineableCallNode>().ToArray();
            var startCallSequence = new List<MethodDefinition>() { instructionNodes[0].Method };
            callNodes.ForEach(x => SetCallNodeProps(x, startCallSequence));
            if (callNodes.Any(x => x.InliningProperties.Recursive))
            {
                foreach (var recursiveCallNode in callNodes.Where(x => x.InliningProperties.Recursive))
                {
                    instructionNodes.First().ProgramFlowBackRoutes.AddTwoWay(recursiveCallNode);
                }
                instructionNodes.ForEach(x => x.InliningProperties.Inlined = true);
            }
            foreach (var callNode in callNodes.Where(x => !x.InliningProperties.Recursive))
            {
                List<InstructionNode> oldCallForwardNodes = callNode.ProgramFlowForwardRoutes.ToList();
                List<InstructionNode> inlinedInstNodes = GetDeepInlineRec(callNode, startCallSequence);
                if (inlinedInstNodes.Count == 0)
                {
                    continue;
                }
                int instWrapperIndex = instructionNodes.IndexOf(callNode);
                instructionNodes.InsertRange(instWrapperIndex + 1, inlinedInstNodes);
                foreach (var inlinedEndNode in inlinedInstNodes.Where(x => x.ProgramFlowForwardRoutes.Count == 0))
                {
                    StitchProgramFlow(inlinedEndNode, oldCallForwardNodes);
                }
            }
        }

        //TODO should merge this with the first function, duplicate code :P
        List<InstructionNode> GetDeepInlineRec(InlineableCallNode callNode, List<MethodDefinition> callSequence)
        {
            List<InstructionNode> oldForwardNodes = callNode.ProgramFlowForwardRoutes.ToList();
            MethodDefinition calledFunc = callNode.CalledFunction;
            var callSequenceClone = new List<MethodDefinition>(callSequence);
            SetCallNodeProps(callNode, callSequenceClone);
            callSequenceClone.Add(calledFunc);
            List<InstructionNode> inlinedNodes = calledFunc.Body.Instructions.Select(x => InstructionNodeFactory.GetInstructionWrapper(x, calledFunc)).ToList();
            programFlowHanlder.AddFlowConnections(inlinedNodes);
            InFuncBackTrace(inlinedNodes);
            StitchProgramFlow(callNode, inlinedNodes[0]);
            List<InlineableCallNode> inlinedCallNodes = inlinedNodes.Where(x => x is InlineableCallNode).Cast<InlineableCallNode>().ToList();
            inlinedNodes.Except(inlinedCallNodes).ForEach(x => x.InliningProperties = callNode.InliningProperties);
            foreach (var nestedCallNode in inlinedCallNodes)
            {
                SetCallNodeProps(nestedCallNode, callSequenceClone);
                if (nestedCallNode.InliningProperties.Recursive)
                {
                    var firstNodeOfThisMethod = inlinedNodes.First();
                    firstNodeOfThisMethod.ProgramFlowBackRoutes.AddTwoWay(nestedCallNode);
                }
                else
                {
                    int nodeIndex = inlinedNodes.IndexOf(nestedCallNode);
                    var inlinedInstNodes = GetDeepInlineRec(nestedCallNode, callSequenceClone);
                    if (inlinedInstNodes.Count > 0)
                    {
                        inlinedNodes.InsertRange(nodeIndex + 1, inlinedInstNodes);
                        foreach (var endNode in inlinedInstNodes.Where(x => x.ProgramFlowForwardRoutes.Count == 0))
                        {
                            StitchProgramFlow(endNode, oldForwardNodes);
                        }
                    }
                }
            }
            return inlinedNodes;
        }

        private void InFuncBackTrace(List<InstructionNode> inlinedNodes)
        {
            LdLocBackTracer locationLoadBackTracer = new LdLocBackTracer(inlinedNodes);
            inlinedNodes.Where(x => x is LocationLoadInstructionNode).ForEach(x => locationLoadBackTracer.AddBackDataflowConnections(x));
        }

        private void SetCallNodeProps(InlineableCallNode callNode, List<MethodDefinition> callSequence)
        {
            callNode.InliningProperties.Inlined = true;
            if (callSequence.Contains(callNode.CalledFunction))
            {
                callNode.InliningProperties.Recursive = true;
                callNode.InliningProperties.RecursionLevel = callSequence.Count(x => x == callNode.CalledFunction);
            }
            else
            {
                callNode.InliningProperties.Recursive = false;
            }
            if (!inlinedCountPerMethod.ContainsKey(callNode.CalledFunction))
            {
                inlinedCountPerMethod.Add(callNode.CalledFunction, 0);
            }
            callNode.InliningProperties.SameMethodCallIndex = inlinedCountPerMethod[callNode.CalledFunction];
            inlinedCountPerMethod[callNode.CalledFunction]++;
        }

        private static void CheckRight(List<InstructionNode> inlinedNodes = null)
        {
            if (inlinedNodes != null && inlinedNodes.Where(x => x.ProgramFlowBackRoutes.Intersect(removedNodes).Count() > 0).ToList().Count > 0)
            {
                throw new Exception("bad bad bad");
            }
            if (MyInstructionNodes.Where(x => x.ProgramFlowBackRoutes.Intersect(removedNodes).Count() > 0).ToList().Count > 0)
            {
                throw new Exception("bad bad bad");
            }
        }

        private static readonly List<InstructionNode> removedNodes = new List<InstructionNode>();

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

        public static void PostBacktraceRecursionStitch(List<InstructionNode> instructionNodes)
        {
            foreach (var recursiveCall in instructionNodes.Where(x => x is InlineableCallNode && x.InliningProperties.Recursive).Cast<InlineableCallNode>().ToList())
            {
                var allRets = instructionNodes.Where(x => x is RetInstructionNode && x.Method == recursiveCall.CalledFunction);
                foreach(var retCall in allRets)
                {
                    retCall.DataFlowForwardRelated.AddTwoWay(recursiveCall.DataFlowForwardRelated);
                }
                recursiveCall.DataFlowForwardRelated.RemoveAllTwoWay();
                foreach (var forwardRoute in recursiveCall.ProgramFlowForwardRoutes.ToArray())
                {
                    forwardRoute.ProgramFlowBackRoutes.AddTwoWay(recursiveCall.ProgramFlowBackRoutes);
                }
                recursiveCall.ProgramFlowForwardRoutes.RemoveAllTwoWay();
                recursiveCall.ProgramFlowBackRoutes.RemoveAllTwoWay();
                recursiveCall.ProgramFlowBackAffected.RemoveAllTwoWay();
                instructionNodes.Remove(recursiveCall);
            }
        }
    }
}
