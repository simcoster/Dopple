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
        ProgramFlowManager programFlowHanlder = new ProgramFlowManager();

        public void Modify(List<InstructionNode> instructionNodes)
        {
            int recursionInstanceIndex = 1;
            foreach (var nestedCallInstWrapper in instructionNodes.Where(x => x is InternalCallInstructionNode).Cast<InternalCallInstructionNode>().ToArray())
            {
                var inlinedInstNodes = GetDeepInlineRec(nestedCallInstWrapper, new List<MethodDefinition>() { instructionNodes[0].Method });
                if (inlinedInstNodes.Count == 0)
                {
                    continue;
                }
                int instWrapperIndex = instructionNodes.IndexOf(nestedCallInstWrapper);
                instructionNodes.InsertRange(instWrapperIndex + 1, inlinedInstNodes);
                foreach (var inlinedInstNode in inlinedInstNodes)
                {
                    inlinedInstNode.InliningProperties.Inlined = true;
                    if (inlinedInstNode is InternalCallInstructionNode)
                    {
                        foreach (var forwardInst in inlinedInstNode.ProgramFlowForwardRoutes.ToList())
                        {
                            forwardInst.ProgramFlowBackRoutes.RemoveTwoWay(inlinedInstNode);
                        }
                        var newNextInstIndex = instructionNodes.IndexOf(inlinedInstNode) + 1;
                        instructionNodes[newNextInstIndex].ProgramFlowBackRoutes.AddTwoWay(inlinedInstNode);
                    }
                }
                if (nestedCallInstWrapper.CalledFunction == nestedCallInstWrapper.Method)
                {
                    foreach(var inst in inlinedInstNodes)
                    {
                        inst.InliningProperties.RecursionInstanceIndex = recursionInstanceIndex;
                        inst.InliningProperties.Recursive = true;
                        if (inst is InternalCallInstructionNode && ((InternalCallInstructionNode) inst).CalledFunction == nestedCallInstWrapper.CalledFunction)
                        {
                            inlinedInstNodes[0].ProgramFlowBackRoutes.AddTwoWay(inst);
                        }
                    }
                    recursionInstanceIndex++;
                }
            }

            foreach (var retCall in instructionNodes.Where(x => x.Instruction.OpCode.Code == Code.Ret && x != instructionNodes.Last()))
            {
                int retCallIndex = instructionNodes.IndexOf(retCall);
                instructionNodes[retCallIndex + 1].ProgramFlowBackRoutes.AddTwoWay(retCall);
                retCall.ProgramFlowResolveDone = true;
                if (retCall.Instruction.OpCode.Code == Code.Ret)
                {
                    if (retCall.Method.ReturnType.FullName == "System.Void")
                    {
                        retCall.StackPushCount = 0;
                    }
                    else
                    {
                        retCall.StackPushCount = 1;
                    }
                }
            }
        }

        List<InstructionNode> GetDeepInlineRec(InternalCallInstructionNode callInstWrapper, List<MethodDefinition> callSequence)
        {
            MethodDefinition calledFunc = callInstWrapper.CalledFunction;
            if (callSequence.Count(x => x == calledFunc) > 1)
            {
                return new List<InstructionNode>();
            }
            else
            {
                var callSequenceClone = new List<MethodDefinition>(callSequence);
                callSequenceClone.Add(calledFunc);
                var calledFuncInstructions = calledFunc.Body.Instructions.ToList();
                var calledFuncInstNodes = calledFuncInstructions.Select(x => InstructionNodeFactory.GetInstructionWrapper(x, calledFunc)).ToList();
                programFlowHanlder.AddFlowConnections(calledFuncInstNodes);
                InFuncBackTrace(calledFuncInstNodes);
                foreach (var nestedCallInstWrapper in calledFuncInstNodes.Where(x => x is InternalCallInstructionNode).Cast<InternalCallInstructionNode>().ToArray())
                {
                    int instWrapperIndex = calledFuncInstNodes.IndexOf(nestedCallInstWrapper);
                    var inlinedInstNodes = GetDeepInlineRec(nestedCallInstWrapper, callSequenceClone);
                    if (inlinedInstNodes.Count > 0)
                    {
                        calledFuncInstNodes.InsertRange(instWrapperIndex + 1, inlinedInstNodes);
                        foreach (var forwardFlowInst in nestedCallInstWrapper.ProgramFlowForwardRoutes.ToList())
                        {
                            forwardFlowInst.ProgramFlowBackRoutes.RemoveTwoWay(nestedCallInstWrapper);
                        }
                        inlinedInstNodes[0].ProgramFlowBackRoutes.AddTwoWay(nestedCallInstWrapper);
                    }
                }
                callInstWrapper.InliningProperties.Inlined = true;
                return calledFuncInstNodes;
            }
        }

        private void InFuncBackTrace(List<InstructionNode> calledFuncInstructions)
        {
            LdLocBackTracer LoadLocationBacktracer = new LdLocBackTracer(calledFuncInstructions);
            foreach(var inst in calledFuncInstructions.Where(x => LoadLocationBacktracer.HandlesCodes.Contains(x.Instruction.OpCode.Code)).OrderByDescending(x => x.InstructionIndex))
            {
                LoadLocationBacktracer.AddBackDataflowConnections(inst);
            }
        }
    }
}
