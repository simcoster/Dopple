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
            for (int i = 0; i < instructionWrappers.Count; i++)
            {
                var instWrapper = instructionWrappers[i];
                if (!(CallOpCodes.Contains(instWrapper.Instruction.OpCode.Code) &&
                            instWrapper.Instruction.Operand is MethodDefinition &&
                            instWrapper.Inlined == false))
                {
                    continue;
                }
                var calledFunc = (MethodDefinition)instWrapper.Instruction.Operand;
                if (calledFunc.FullName == instWrapper.Method.FullName)
                {
                    continue;
                }
                instWrapper.Inlined = true;
                AddInlinedMethodBody(instructionWrappers, i + 1, calledFunc);
                int addedInstrucitons = StArgAdder.InsertHelperSTargs(instructionWrappers, instWrapper, calledFunc).Count;
                i += addedInstrucitons;
            }
        }

       

        private static void AddInlinedMethodBody(List<InstructionWrapper> instructionWrappers, int indexForAddingInlined, MethodDefinition calledFunc)
        {
            var calledFuncInstructions = calledFunc.Body.Instructions.ToList();
            var calledFunInstWrappers = calledFuncInstructions.Select(x => new InstructionWrapper(x, calledFunc)).ToList();
            instructionWrappers.InsertRange(indexForAddingInlined, calledFunInstWrappers);
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

     
    }
}
