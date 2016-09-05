using System.Collections.Generic;
using System.Linq;
using DoppleTry2.InstructionModifiers;
using Mono.Cecil.Cil;

namespace DoppleTry2.ProgramFlowHanlder
{
    class SimpleForwardPathHanlder : ForwardPathHanlder
    {
        public SimpleForwardPathHanlder(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override Code[] HandledCodes => new[] {Code.Br_S,};
        protected override void SetForwardExecutionFlowInsts(InstructionWrapper instructionWrapper)
        {
            InstructionWrapper nextInstruction =
                InstructionsWrappers.First(x => x.Instruction == instructionWrapper.Instruction.Operand);
            TwoWayLinkExecutionPath(instructionWrapper,nextInstruction);
        }
    }
}