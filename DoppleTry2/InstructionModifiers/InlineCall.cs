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
            foreach (var callNode in instructionNodes.Where(x => x is InternalCallInstructionNode).Cast<InternalCallInstructionNode>().ToArray())
            {
                List<InstructionNode> oldCallForwardNodes = callNode.ProgramFlowForwardRoutes;
                List<InstructionNode> inlinedInstNodes = GetDeepInlineRec(callNode, new List<MethodDefinition>() { instructionNodes[0].Method });
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
        List<InstructionNode> GetDeepInlineRec(InternalCallInstructionNode callNode, List<MethodDefinition> callSequence)
        {
            List<InstructionNode> oldForwardNodes = callNode.ProgramFlowForwardRoutes;
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
                InFuncBackTrace(calledFuncInstNodes);
                if (calledFuncInstNodes.Count > 0)
                {
                    StitchProgramFlow(callNode, calledFuncInstNodes[0]);
                }
                foreach (var nestedCallNode in calledFuncInstNodes.Where(x => x is InternalCallInstructionNode).Cast<InternalCallInstructionNode>().ToArray())
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
                        calledFuncInstNodes[0].ProgramFlowBackRoutes.AddTwoWay(nestedCallNode);
                    }
                }
                callNode.InliningProperties.Inlined = true;
                return calledFuncInstNodes;
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

        private void InFuncBackTrace(List<InstructionNode> calledFuncInstructions)
        {
            var loadLocationBacktracer = new LdLocBackTracer(calledFuncInstructions);
            foreach(var inst in calledFuncInstructions.Where(x => loadLocationBacktracer.HandlesCodes.Contains(x.Instruction.OpCode.Code)).OrderByDescending(x => x.InstructionIndex))
            {
                loadLocationBacktracer.AddBackDataflowConnections(inst);
            }
        }
    }
}
