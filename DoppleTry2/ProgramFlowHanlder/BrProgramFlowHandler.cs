using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;

namespace DoppleTry2.ProgramFlowHanlder
{
    class BrProgramFlowHandler : ProgramFlowHandler
    {
        public BrProgramFlowHandler(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        public override Code[] HandledCodes => new[] {Code.Br_S, Code.Br};
        protected override void SetForwardExecutionFlowInstsInternal(InstructionWrapper instructionWrapper)
        {
            InstructionWrapper nextInstruction =
                InstructionWrappers.First(x => x.Instruction == instructionWrapper.Instruction.Operand);
            TwoWayLinkExecutionPath(instructionWrapper,nextInstruction);
        }
    }
}