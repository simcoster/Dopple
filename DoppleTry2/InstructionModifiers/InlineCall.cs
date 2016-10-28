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
        public void Modify(List<InstructionWrapper> instructionWrappers)
        {
            foreach (var nestedCallInstWrapper in instructionWrappers.Where(x => x is CallInstructionWrapper).ToArray())
            {
                int instWrapperIndex = instructionWrappers.IndexOf(nestedCallInstWrapper);
                var inlinedInstWrappers = GetDeepInlineRec(nestedCallInstWrapper, new List<MethodDefinition>());
                if (inlinedInstWrappers.Count > 0)
                {
                    instructionWrappers.InsertRange(instWrapperIndex + 1, inlinedInstWrappers);
                    nestedCallInstWrapper.NextPossibleProgramFlow.Clear();
                    ProgramFlowHandler.TwoWayLinkExecutionPath(nestedCallInstWrapper, inlinedInstWrappers[0]);
                }
            }
            foreach (var nestedCallInstWrapper in instructionWrappers.Where(x => x is CallInstructionWrapper).Cast<CallInstructionWrapper>().ToArray())
            {
                StArgAdder.InsertHelperSTargs(instructionWrappers, nestedCallInstWrapper);
            }
        }

        List<InstructionWrapper> GetDeepInlineRec(InstructionWrapper callInstWrapper, List<MethodDefinition> callSequence)
        {
            if (!(callInstWrapper is CallInstructionWrapper))
            {
                throw new Exception("only user functions should be inlined");
            }
            var calledFunc = (MethodDefinition)callInstWrapper.Instruction.Operand;
            if (callSequence.Contains(calledFunc))
            {
                return new List<InstructionWrapper>();
            }
            else
            {
                var callSequenceClone = new List<MethodDefinition>(callSequence);
                callSequenceClone.Add(calledFunc);
                var calledFuncInstructions = calledFunc.Body.Instructions.ToList();
                var calledFuncInstWrappers = calledFuncInstructions.Select(x => InstructionWrapperFactory.GetInstructionWrapper(x, calledFunc)).ToList();
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
                return calledFuncInstWrappers;
            }
        }
    }
}
