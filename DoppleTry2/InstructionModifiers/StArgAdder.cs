using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.InstructionModifiers
{
    public static class StArgAdder
    {
        public static List<InstructionWrapper> InsertHelperSTargs(List<InstructionWrapper> instructionWrappers, InstructionWrapper instWrapper, MethodDefinition calledFunc)
        {
            var addedInstructions = new List<InstructionWrapper>();
            for (int i = calledFunc.Parameters.Count - 1; i >= 0; i--)
            {
                var argProvidingWrappers = BackSearcher.SearchBackwardsForDataflowInstrcutions(instructionWrappers, x => x.StackPushCount > 0, instWrapper);
                foreach (var argProvidingWrapper in argProvidingWrappers)
                {
                    var opcode = Instruction.Create(OpCodes.Starg, calledFunc.Parameters[i]);
                    InstructionWrapper stArgWrapper = new InstructionWrapper(opcode, instWrapper.Method);
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
                    stArgWrapper.Inlined = true;
                    argProvidingWrapper.StackPushCount--;
                    instructionWrappers.Insert(instructionWrappers.IndexOf(argProvidingWrapper) + 1, stArgWrapper);
                    addedInstructions.Add(stArgWrapper);
                }
            }
            return addedInstructions;
        }
    }
}
