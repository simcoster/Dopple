using DoppleTry2.BackTrackers;
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
            var stackPopBacktracer = new StackPopBackTracer(instructionWrappers);
            int maxArgIndex;
            int minArgIndex;
            if (calledFunc.IsStatic)
            {
                maxArgIndex = calledFunc.Parameters.Count -1;
                minArgIndex = 0;
            }
            else
            {
                maxArgIndex = calledFunc.Parameters.Count;
                minArgIndex = 1;
            }
            for (int i = maxArgIndex; i >= minArgIndex; i--)
            {
                //var backSearcher = new SingleIndexBackSearcher(instructionWrappers);
                //var argProvidingWrappers = backSearcher.SearchBackwardsForDataflowInstrcutions(x => x.StackPushCount > 0, callInstWrapper);
                var argProvidingWrappers = stackPopBacktracer.SearchAndAddDataflowInstrcutions(callInstWrapper);
                foreach (var argProvidingWrapper in argProvidingWrappers)
                {
                    Instruction opcode = Instruction.Create(OpCodes.Starg, calledFunc.Parameters[i - 1]);
                    var stArgWrapper = (StArgInstructionWrapper)InstructionWrapperFactory.GetInstructionWrapper(opcode, calledFunc);
                    AddStArgInst(instructionWrappers, addedInstructions, calledFunc, i, argProvidingWrapper, stArgWrapper);
                }
            }
            if (!calledFunc.IsStatic)
            {
                var argProvidingWrappers = stackPopBacktracer.SearchAndAddDataflowInstrcutions(callInstWrapper);
                foreach (var argProvidingWrapper in argProvidingWrappers)
                {
                    Instruction opcode = Instruction.Create(OpCodes.Starg, calledFunc.Parameters[0]);
                    var stArgWrapper = new StThisArgInstructionWrapper(opcode, calledFunc);
                    AddStArgInst(instructionWrappers, addedInstructions, calledFunc, 0, argProvidingWrapper, stArgWrapper);
                }
            }
            return addedInstructions;
        }

        private static void AddStArgInst(List<InstructionWrapper> instructionWrappers, List<InstructionWrapper> addedInstructions, MethodDefinition calledFunc, int i, InstructionWrapper argProvidingWrapper, StArgInstructionWrapper stArgWrapper)
        {
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
            stArgWrapper.ProgramFlowResolveDone = true;
            argProvidingWrapper.StackPushCount--;
            instructionWrappers.Insert(instructionWrappers.IndexOf(argProvidingWrapper) + 1, stArgWrapper);
            addedInstructions.Add(stArgWrapper);
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
