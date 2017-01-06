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
            var callSequenceClone = new List<MethodDefinition>(callSequence);
            callSequenceClone.Add(calledFunc);
            List<InstructionNode> calledFuncInstNodes = calledFunc.Body.Instructions.Select(x => InstructionNodeFactory.GetInstructionWrapper(x, calledFunc)).ToList();
            calledFuncInstNodes.ForEach(x => x.InliningProperties.Inlined = true);
            programFlowHanlder.AddFlowConnections(calledFuncInstNodes);
            StitchProgramFlow(callNode, calledFuncInstNodes[0]);
            List<InlineableCallNode> allCallNodes = calledFuncInstNodes.Where(x => x is InlineableCallNode).Cast<InlineableCallNode>().ToList();
            var callsToExpand = allCallNodes.Where(x => !callSequence.Contains(x.CalledFunction)).ToList();
            foreach (var nestedCallNode in callsToExpand)
            {
                int nodeIndex = calledFuncInstNodes.IndexOf(nestedCallNode);
                var inlinedInstNodes = GetDeepInlineRec(nestedCallNode, callSequenceClone);
                if (inlinedInstNodes.Count > 0)
                {
                    calledFuncInstNodes.InsertRange(nodeIndex + 1, inlinedInstNodes);
                    foreach (var endNode in inlinedInstNodes.Where(x => x.ProgramFlowForwardRoutes.Count == 0))
                    {
                        StitchProgramFlow(endNode, oldForwardNodes);
                    }
                }
            }
            foreach(var recursiveCalls in allCallNodes.Where(x => callSequence.Contains(x.CalledFunction)))
            {
                recursiveCalls.InliningProperties.Recursive = true;
                var firstNodeOfThisMethod = calledFuncInstNodes.First(x => x.Method == recursiveCalls.CalledFunction);
                firstNodeOfThisMethod.ProgramFlowBackRoutes.AddTwoWay(recursiveCalls);
            }
            callNode.InliningProperties.Inlined = true;
            return calledFuncInstNodes;
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
