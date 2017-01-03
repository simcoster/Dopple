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

        public void Modify(List<InstructionNode> instructionNodes)
        {
            int recursionInstanceIndex = 1;
            foreach (var callNode in instructionNodes.Where(x => x is InlineableCallNode).Cast<InlineableCallNode>().ToArray())
            {
                List<InstructionNode> oldCallForwardNodes = callNode.ProgramFlowForwardRoutes.ToList();
                List<InstructionNode> inlinedInstNodes = GetDeepInlineRec(callNode, new List<MethodDefinition>() { instructionNodes[0].Method });
                if (inlinedInstNodes.Count == 0)
                {
                    continue;
                }
                foreach (var inst in inlinedInstNodes)
                {
                    inst.InliningProperties.Inlined = true;
                }
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
                        inst.InliningProperties.RecursionInstanceIndex = recursionInstanceIndex;
                        inst.InliningProperties.Recursive = true;
                    }
                    recursionInstanceIndex++;
                }
            }
        }



        //TODO this is all messed up, needs to be changed
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
                programFlowHanlder.AddFlowConnections(calledFuncInstNodes);
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
                        }
                    }
                    else if (nestedCallNode.CalledFunction == callNode.CalledFunction)
                    {
                        var oldForwardPaths = nestedCallNode.ProgramFlowForwardRoutes.ToList();
                        StitchProgramFlow(nestedCallNode, calledFuncInstNodes[0]);
                        IEnumerable<InstructionNode> inaccessiblePaths = oldForwardPaths.Where(x => x.ProgramFlowBackRoutes.Count == 0);
                        foreach(var inaccessbilePath in inaccessiblePaths)
                        {
                            RemovePathUntilJunction(calledFuncInstNodes, inaccessbilePath);
                        }
                    }
                }
                callNode.InliningProperties.Inlined = true;
                return calledFuncInstNodes;
            }
        }

        private NonInlineableCallInstructionNode ReplaceWithNonInlineable(InlineableCallNode nodeToReplace, List<InstructionNode> calledFuncInstNodes)
        {
            var clonedNode = new NonInlineableCallInstructionNode(nodeToReplace.Instruction, nodeToReplace.Method);
            foreach(var backNode in nodeToReplace.ProgramFlowBackRoutes.ToList())
            {
                clonedNode.ProgramFlowBackRoutes.AddTwoWay(backNode);
                nodeToReplace.ProgramFlowBackRoutes.RemoveTwoWay(backNode);
            }
            foreach(var forwardNode in nodeToReplace.ProgramFlowForwardRoutes.ToList())
            {
                forwardNode.ProgramFlowBackRoutes.AddTwoWay(clonedNode);
                forwardNode.ProgramFlowBackRoutes.RemoveTwoWay(nodeToReplace);
            }
            calledFuncInstNodes[calledFuncInstNodes.IndexOf(nodeToReplace)] = clonedNode;
            return clonedNode;
        }

        private static void RemovePathUntilJunction(List<InstructionNode> calledFuncInstNodes, InstructionNode inaccessbileNode)
        {
            var currentNode = inaccessbileNode;
            if (inaccessbileNode.ProgramFlowBackRoutes.Count < 2)
            {
                calledFuncInstNodes.Remove(inaccessbileNode);
                foreach(var nextNode in inaccessbileNode.ProgramFlowForwardRoutes)
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
