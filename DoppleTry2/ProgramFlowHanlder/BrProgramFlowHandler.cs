using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;

namespace DoppleTry2.ProgramFlowHanlder
{
    class BrProgramFlowHandler : ProgramFlowHandler
    {
        public override Code[] HandledCodes => new[] {Code.Br_S, Code.Br};

        protected override void SetForwardExecutionFlowInstsInternal(InstructionWrapper wrapperToModify, List<InstructionWrapper> instructionWrappers)
        {
            InstructionWrapper nextInstruction =
             instructionWrappers.First(x => x.Instruction == wrapperToModify.Instruction.Operand);
            TwoWayLinkExecutionPath(wrapperToModify, nextInstruction);
        }
    }
}