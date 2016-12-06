using System;
using System.Collections.Generic;
using System.Linq;
using DoppleTry2.InstructionModifiers;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionWrappers;

namespace DoppleTry2.ProgramFlowHanlder
{
    class SimpleProgramFlowHandler : ProgramFlowHandler
    {
        public SimpleProgramFlowHandler()
        {
            HandledCodes = typeof(OpCodes).GetFields()
                .Select(x => x.GetValue(null))
                .Cast<OpCode>()
                .Select(y => y.Code).
                Except(_unhandledCodes).ToArray();
        }

        private readonly Code[] _unhandledCodes = new[] { Code.Br, Code.Br_S };

        public override Code[] HandledCodes { get; }

        public override void SetForwardExecutionFlowInsts(InstructionWrapper wrapperToModify, List<InstructionWrapper> instructionWrappers)
        {
            InstructionWrapper nextInstructionWrapper =
               instructionWrappers.FirstOrDefault(x => x.Instruction == wrapperToModify.Instruction.Next);
            if (nextInstructionWrapper == null)
            {
                return;
            }
            nextInstructionWrapper.BackProgramFlow.AddTwoWay(wrapperToModify);
        }
    }
}