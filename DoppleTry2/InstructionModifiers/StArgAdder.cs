using DoppleTry2.InstructionWrappers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.InstructionModifiers
{
    public class StArgAdder : IModifier
    {
        private static List<InstructionWrapper> InsertHelperSTargs(List<InstructionWrapper> instructionWrappers, CallInstructionWrapper callInstWrapper)
        {
            var addedInstructions = new List<InstructionWrapper>();
            var calledFunc = callInstWrapper.CalledFunction;

            for (int i = calledFunc.Parameters.Count - 1; i >= 0; i--)
            {
                var argProvidingWrappers = BackSearcher.SearchBackwardsForDataflowInstrcutions(instructionWrappers, x => x.StackPushCount > 0, callInstWrapper);
                foreach (var argProvidingWrapper in argProvidingWrappers)
                {
                    var opcode = Instruction.Create(OpCodes.Starg, calledFunc.Parameters[i]);
                    StArgInstructionWrapper stArgWrapper = (StArgInstructionWrapper)InstructionWrapperFactory.GetInstructionWrapper(opcode, calledFunc);
                    stArgWrapper.Instruction.Offset = 99999;
                    stArgWrapper.BackProgramFlow.Add(argProvidingWrapper);
                    stArgWrapper.NextPossibleProgramFlow.AddRange(argProvidingWrapper.NextPossibleProgramFlow);
                    foreach (var nextPossiblePrFlow in stArgWrapper.NextPossibleProgramFlow)
                    {
                        nextPossiblePrFlow.BackProgramFlow.Remove(argProvidingWrapper);
                        nextPossiblePrFlow.BackProgramFlow.Add(stArgWrapper);
                    }
                    argProvidingWrapper.NextPossibleProgramFlow.Clear();
                    argProvidingWrapper.NextPossibleProgramFlow.Add(stArgWrapper);
                    stArgWrapper.AddBackDataflowTwoWaySingleIndex(new[] { argProvidingWrapper });
                    stArgWrapper.StackPopCount--;
                    stArgWrapper.ArgIndex = i;
                    if (!calledFunc.IsStatic)
                    {
                        stArgWrapper.ArgIndex--;
                    }
                    stArgWrapper.ProgramFlowResolveDone = true;
                    argProvidingWrapper.StackPushCount--;
                    instructionWrappers.Insert(instructionWrappers.IndexOf(argProvidingWrapper) + 1, stArgWrapper);
                    addedInstructions.Add(stArgWrapper);
                }
            }
            return addedInstructions;
        }

        public void Modify(List<InstructionWrapper> instructionWrappers)
        {
            var callInstructions = instructionWrappers
                                    .Where(x => x is CallInstructionWrapper && x.InliningProperties.Inlined)
                                    .OrderByDescending(x => instructionWrappers.IndexOf(x))
                                    .Cast<CallInstructionWrapper>()
                                    .ToArray();
            foreach (var callInst in callInstructions)
            {
                InsertHelperSTargs(instructionWrappers, callInst);
            }
        }
    }
}
