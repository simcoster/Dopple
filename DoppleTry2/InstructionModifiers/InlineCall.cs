using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using DoppleTry2.BackTrackers;
using DoppleTry2.ProgramFlowHanlder;

namespace DoppleTry2.InstructionModifiers
{
    class InlineCallModifier : IPreBacktraceModifier
    {
        public static readonly Code[] CallOpCodes = CodeGroups.CallCodes;
        ProgramFlowManager programFlowHanlder = new ProgramFlowManager();

        public void Modify(List<InstructionWrapper> instructionWrappers)
        {
            foreach (var nestedCallInstWrapper in instructionWrappers.Where(x => x is CallInstructionWrapper).Cast<CallInstructionWrapper>().ToArray())
            {
                var inlinedInstWrappers = GetDeepInlineRec(nestedCallInstWrapper, new List<MethodDefinition>());
                if (inlinedInstWrappers.Count > 0)
                {
                    int instWrapperIndex = instructionWrappers.IndexOf(nestedCallInstWrapper);
                    instructionWrappers.InsertRange(instWrapperIndex + 1, inlinedInstWrappers);
                }
                foreach (var inlinedLdArg in inlinedInstWrappers.Where(x => CodeGroups.LdArgCodes.Contains(x.Instruction.OpCode.Code)))
                {
                    inlinedLdArg.Inlined = true;
                }
            }

            foreach (var nestedCallInstWrapper in instructionWrappers.Where(x => x is CallInstructionWrapper && x.Inlined))
            {
                int index = instructionWrappers.IndexOf(nestedCallInstWrapper);
                ProgramFlowHandler.TwoWayLinkExecutionPath(nestedCallInstWrapper, instructionWrappers[index+1]);
                nestedCallInstWrapper.ProgramFlowResolveDone = true;
            }

            foreach (var retCall in instructionWrappers.Where(x => x.Instruction.OpCode.Code == Code.Ret && x != instructionWrappers.Last()))
            {
                int index = instructionWrappers.IndexOf(retCall);
                ProgramFlowHandler.TwoWayLinkExecutionPath(retCall, instructionWrappers[index + 1]);
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

        List<InstructionWrapper> GetDeepInlineRec(InstructionWrapper callInstWrapper, List<MethodDefinition> callSequence)
        {
            if (!(callInstWrapper is CallInstructionWrapper))
            {
                throw new Exception("only user functions should be inlined");
            }
            var calledFunc = (MethodDefinition)callInstWrapper.Instruction.Operand;
            if (callSequence.Count(x => x == calledFunc) > 0)
            {
                return new List<InstructionWrapper>();
            }
            else
            {
                var callSequenceClone = new List<MethodDefinition>(callSequence);
                callSequenceClone.Add(calledFunc);
                var calledFuncInstructions = calledFunc.Body.Instructions.ToList();
                var calledFuncInstWrappers = calledFuncInstructions.Select(x => InstructionWrapperFactory.GetInstructionWrapper(x, calledFunc)).ToList();
                programFlowHanlder.AddFlowConnections(calledFuncInstWrappers);
                foreach (var nestedCallInstWrapper in calledFuncInstWrappers.Where(x => x is CallInstructionWrapper).ToArray())
                {
                    int instWrapperIndex = calledFuncInstWrappers.IndexOf(nestedCallInstWrapper);
                    var inlinedInstWrappers = GetDeepInlineRec(nestedCallInstWrapper, callSequenceClone);
                    if (inlinedInstWrappers.Count > 0)
                    {
                        calledFuncInstWrappers.InsertRange(instWrapperIndex + 1, inlinedInstWrappers);
                        nestedCallInstWrapper.NextPossibleProgramFlow.Clear();
                        ProgramFlowHandler.TwoWayLinkExecutionPath(nestedCallInstWrapper, inlinedInstWrappers[0]);
                    }
                }
                callInstWrapper.Inlined = true;
                return calledFuncInstWrappers;
            }
        }
    }
}
