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
    class InlineCall : IModifier
    {
        public static readonly Code[] CallOpCodes = { Code.Call, Code.Calli, Code.Callvirt };

        public void Modify(List<InstructionWrapper> instructionWrappers)
        {
            for (int i = 0; i < instructionWrappers.Count; i++)
            {
                var instWrapper = instructionWrappers[i];
                if (!(CallOpCodes.Contains(instWrapper.Instruction.OpCode.Code) &&
                            instWrapper.Instruction.Operand is MethodDefinition &&
                            instWrapper.Inlined == false))
                    continue;
                //instructionWrappers.Remove(instWrapper);
                var calledFunc = (MethodDefinition)instWrapper.Instruction.Operand;
                if (calledFunc.FullName == instWrapper.Method.FullName)
                    continue;
                AddInlinedMethodBody(instructionWrappers, i + 1, calledFunc);
                instWrapper.Inlined = true;
                int addedInstrucitons = InsertHelperSTargs(instructionWrappers, instWrapper, calledFunc);
                i += addedInstrucitons;
            }
        }

        private static void AddInlinedMethodBody(List<InstructionWrapper> instructionWrappers, int i, MethodDefinition calledFunc)
        {
            var calledFuncInstructions = calledFunc.Body.Instructions.ToList();
            var calledFunInstWrappers = calledFuncInstructions.Select(x => new InstructionWrapper(x, calledFunc)).ToList();
            instructionWrappers.InsertRange(i, calledFunInstWrappers);
            var programFlowHelper = new SimpleProgramFlowHandler(instructionWrappers);
            var lastInlinedCallIndex = instructionWrappers.IndexOf(calledFunInstWrappers.Last());
            foreach (var wrapper in calledFunInstWrappers)
            {
                wrapper.Inlined = true;
                wrapper.Method = calledFunc;
                if (wrapper.Instruction.OpCode.Code == Code.Ret)
                {
                    wrapper.StackPushCount = 1;
                    wrapper.NextPossibleProgramFlow.Clear();
                    programFlowHelper.TwoWayLinkExecutionPath(wrapper, instructionWrappers[lastInlinedCallIndex + 1]);
                }
            }
        }

        private static int InsertHelperSTargs(List<InstructionWrapper> instructionWrappers, InstructionWrapper instWrapper, MethodDefinition calledFunc)
        {
            int addedInstructions = 0;
            for (int i = 0; i < calledFunc.Parameters.Count; i++)
            {
                var argProvidingWrappers = BackSearcher.SearchBackwardsForDataflowInstrcutions(instructionWrappers, x => x.StackPushCount > 0, instWrapper);
                foreach (var argProvidingWrapper in argProvidingWrappers)
                {
                    addedInstructions++;
                    var opcode = Instruction.Create(OpCodes.Starg, calledFunc.Parameters[i]);
                    InstructionWrapper stArgWrapper = new InstructionWrapper(opcode, calledFunc);
                    stArgWrapper.BackProgramFlow.Add(argProvidingWrapper);
                    stArgWrapper.NextPossibleProgramFlow.AddRange(argProvidingWrapper.NextPossibleProgramFlow);
                    foreach(var nextPossiblePrFlow in stArgWrapper.NextPossibleProgramFlow)
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
                    instructionWrappers.Insert(instructionWrappers.IndexOf(argProvidingWrapper) +1, stArgWrapper);
                }
            }
            return addedInstructions;
        }
    }
}
