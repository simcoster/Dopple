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

namespace DoppleTry2.InstructionModifiers
{
    class InlineCallModifier : IPreBacktraceModifier
    {
        public static readonly Code[] CallOpCodes = CodeGroups.CallCodes;
        readonly ProgramFlowManager programFlowHanlder = new ProgramFlowManager();
        private static List<InstructionNode> MyInstructionNodes;

        public void Modify(List<InstructionNode> instructionNodes)
        {
            MyInstructionNodes = instructionNodes;
            int recursionInstanceIndex = 1;
            foreach (var callNode in instructionNodes.Where(x => x is InlineableCallNode).Cast<InlineableCallNode>().ToArray())
            {
                List<InstructionNode> oldCallForwardNodes = callNode.ProgramFlowForwardRoutes.ToList();
                List<InstructionNode> inlinedInstNodes = GetDeepInlineRec(callNode, new List<MethodDefinition>() { instructionNodes[0].Method });
               if (inlinedInstNodes.Count == 0)
                {
                    continue;
                }
                inlinedInstNodes.ForEach(x => x.InliningProperties.Inlined = true);
                int instWrapperIndex = instructionNodes.IndexOf(callNode);
                instructionNodes.InsertRange(instWrapperIndex + 1, inlinedInstNodes);
                foreach (var inlinedEndNode in inlinedInstNodes.Where(x => x.ProgramFlowForwardRoutes.Count == 0))
                {
                    StitchProgramFlow(inlinedEndNode, oldCallForwardNodes);
                }
                if (callNode.CalledFunction == callNode.Method)
                {
                    foreach(var inst in inlinedInstNodes)
                    {
                        inst.InliningProperties.RecursionSameLevelIndex = recursionInstanceIndex;
                        inst.InliningProperties.Recursive = true;
                    }
                    recursionInstanceIndex++;
                }
            }
            RemoveOraphanedNodes(MyInstructionNodes);
        }



        //TODO should merge this with the first function, duplicate code :P
        List<InstructionNode> GetDeepInlineRec(InlineableCallNode callNode, List<MethodDefinition> callSequence)
        {
            List<InstructionNode> oldForwardNodes = callNode.ProgramFlowForwardRoutes.ToList();
            MethodDefinition calledFunc = callNode.CalledFunction;
            if (callSequence.Count(x => x == calledFunc) > 1)
            {
                return new List<InstructionNode>();
            }
            else
            {
                var callSequenceClone = new List<MethodDefinition>(callSequence);
                callSequenceClone.Add(calledFunc);
                List<InstructionNode> calledFuncInstNodes = calledFunc.Body.Instructions.Select(x => InstructionNodeFactory.GetInstructionWrapper(x, calledFunc)).ToList();
                calledFuncInstNodes.ForEach(x => x.InliningProperties.Inlined = true);
                programFlowHanlder.AddFlowConnections(calledFuncInstNodes);
                if (calledFuncInstNodes.Count > 0)
                {
                    StitchProgramFlow(callNode, calledFuncInstNodes[0]);
                }
                var nestedCalls = calledFuncInstNodes.Where(x => x is InlineableCallNode).Cast<InlineableCallNode>().ToArray();
                foreach (var nestedCallNode in nestedCalls.Where(x => calledFuncInstNodes.Contains(x)))
                {
                    int instWrapperIndex = calledFuncInstNodes.IndexOf(callNode);
                    var inlinedInstNodes = GetDeepInlineRec(callNode, callSequenceClone);
                    if (inlinedInstNodes.Count > 0)
                    {
                        calledFuncInstNodes.InsertRange(instWrapperIndex + 1, inlinedInstNodes);
                        foreach(var endNode in inlinedInstNodes.Where(x => x.ProgramFlowForwardRoutes.Count == 0))
                        {
                            StitchProgramFlow(endNode, oldForwardNodes);
                        }
                    }
                    else if (nestedCallNode.CalledFunction == callNode.CalledFunction)
                    {
                        var oldForwardPaths = nestedCallNode.ProgramFlowForwardRoutes.ToList();
                        calledFuncInstNodes[0].ProgramFlowBackRoutes.AddTwoWay(nestedCallNode);
                        if (callSequenceClone.Count(x => x == nestedCallNode.CalledFunction) > 1)
                        {
                            nestedCallNode.InliningProperties.RecursionLevel = 2;
                        }
                        else
                        {
                            nestedCallNode.InliningProperties.RecursionLevel = 1;
                        }
                    }
                }
                callNode.InliningProperties.Inlined = true;
                return calledFuncInstNodes;
            }
        }

        private static void CheckRight(List<InstructionNode> inlinedNodes = null)
        {
            if (inlinedNodes!=null && inlinedNodes.Where(x => x.ProgramFlowBackRoutes.Intersect(removedNodes).Count() > 0).ToList().Count > 0)
            {
                throw new Exception("bad bad bad");
            }
            if (MyInstructionNodes.Where(x => x.ProgramFlowBackRoutes.Intersect(removedNodes).Count() > 0).ToList().Count > 0)
            {
                throw new Exception("bad bad bad");
            }
        }

        private static List<InstructionNode> removedNodes = new List<InstructionNode>();
        private static void RemoveOraphanedNodes(List<InstructionNode> instructionNodes)
        {
            var accessibleNodes =BackSearcher.GetForwardFlowTree(MyInstructionNodes[0]);
            foreach(var inacessibleNode in MyInstructionNodes.Except(accessibleNodes).ToArray())
            {
                instructionNodes.Remove(inacessibleNode);
                inacessibleNode.ProgramFlowForwardRoutes.RemoveAllTwoWay();
                inacessibleNode.ProgramFlowBackRoutes.RemoveAllTwoWay();
            }
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
            foreach(var newForwardNode in forwardNodes)
            {
                newForwardNode.ProgramFlowBackRoutes.AddTwoWay(backNode);
            }
        }
    }
}
