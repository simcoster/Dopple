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
                CheckRight();
                if (inlinedInstNodes.Count == 0)
                {
                    continue;
                }
                foreach (var inst in inlinedInstNodes)
                {
                    inst.InliningProperties.Inlined = true;
                }
                CheckRight();
                int instWrapperIndex = instructionNodes.IndexOf(callNode);
                CheckRight();

                instructionNodes.InsertRange(instWrapperIndex + 1, inlinedInstNodes);
                CheckRight();
                foreach (var inlinedEndNode in inlinedInstNodes.Where(x => x.ProgramFlowForwardRoutes.Count == 0))
                {
                    StitchProgramFlow(inlinedEndNode, oldCallForwardNodes);
                    CheckRight();
                }
                if (callNode.CalledFunction == callNode.Method)
                {
                    foreach(var inst in inlinedInstNodes)
                    {
                        inst.InliningProperties.RecursionInstanceIndex = recursionInstanceIndex;
                        inst.InliningProperties.Recursive = true;
                        CheckRight();
                    }
                    recursionInstanceIndex++;
                }
            }
        }



        //TODO this is all messed up, needs to be changed
        List<InstructionNode> GetDeepInlineRec(InlineableCallNode callNode, List<MethodDefinition> callSequence)
        {
            CheckRight();
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
                CheckRight();
                programFlowHanlder.AddFlowConnections(calledFuncInstNodes);
                CheckRight();
                if (calledFuncInstNodes.Count > 0)
                {
                    StitchProgramFlow(callNode, calledFuncInstNodes[0]);
                }
                foreach (var nestedCallNode in calledFuncInstNodes.Where(x => x is InlineableCallNode).Cast<InlineableCallNode>().ToArray())
                {
                    int instWrapperIndex = calledFuncInstNodes.IndexOf(callNode);
                    var inlinedInstNodes = GetDeepInlineRec(callNode, callSequenceClone);
                    if (inlinedInstNodes.Count > 0)
                    {
                        calledFuncInstNodes.InsertRange(instWrapperIndex + 1, inlinedInstNodes);
                        foreach(var endNode in inlinedInstNodes.Where(x => x.ProgramFlowForwardRoutes.Count == 0))
                        {
                            StitchProgramFlow(endNode, oldForwardNodes);
                            CheckRight();
                        }
                    }
                    else if (nestedCallNode.CalledFunction == callNode.CalledFunction)
                    {
                        CheckRight();
                        var oldForwardPaths = nestedCallNode.ProgramFlowForwardRoutes.ToList();
                        StitchProgramFlow(nestedCallNode, calledFuncInstNodes[0]);
                        IEnumerable<InstructionNode> inaccessiblePaths = oldForwardPaths.Where(x => x.ProgramFlowBackRoutes.Count == 0).ToList();
                        foreach (var inaccessbilePath in inaccessiblePaths)
                        {
                            // TODO check
                            RemovePathUntilJunction(calledFuncInstNodes, inaccessbilePath);
                            CheckRight();
                        }
                    }
                }
                callNode.InliningProperties.Inlined = true;
                CheckRight();
                return calledFuncInstNodes;
            }
        }

        private static void CheckRight()
        {
            if (MyInstructionNodes.Where(x => x.ProgramFlowBackRoutes.Intersect(removedNodes).Count() > 0).ToList().Count > 0)
            {
                throw new Exception("bad bad bad");
            }
        }

        private static List<InstructionNode> removedNodes = new List<InstructionNode>();
        private static void RemovePathUntilJunction(List<InstructionNode> calledFuncInstNodes, InstructionNode inaccessbileNode)
        {
            var currentNode = inaccessbileNode;
            removedNodes.Add(inaccessbileNode);
            if (inaccessbileNode.ProgramFlowBackRoutes.Count < 2)
            {
                calledFuncInstNodes.Remove(inaccessbileNode);
                var forwardNodes = inaccessbileNode.ProgramFlowForwardRoutes.ToList();
                inaccessbileNode.ProgramFlowForwardRoutes.RemoveAllTwoWay();
                inaccessbileNode.ProgramFlowBackRoutes.RemoveAllTwoWay();
                var stillReferenced1 = MyInstructionNodes.Where(x => x.ProgramFlowBackRoutes.Intersect(removedNodes).Count() > 0).ToList();
                var stillReferenced2 = calledFuncInstNodes.Where(x => x.ProgramFlowBackRoutes.Intersect(removedNodes).Count() > 0).ToList();

                if (stillReferenced1.Count > 0 || stillReferenced2.Count > 0)
                {
                    throw new Exception("bad bad bad");
                }
                else
                {

                }
                foreach (var nextNode in forwardNodes)
                {
                    RemovePathUntilJunction(calledFuncInstNodes, nextNode);
                }
           
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
