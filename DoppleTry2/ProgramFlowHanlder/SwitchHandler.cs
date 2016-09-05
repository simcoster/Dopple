using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionModifiers;
using Mono.Cecil.Cil;

namespace DoppleTry2.ProgramFlowHanlder
{
    class SwitchHandler : ForwardPathHanlder
    {
        public SwitchHandler(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override Code[] HandledCodes => new[] {Code.Switch};
        protected override void SetForwardExecutionFlowInsts(InstructionWrapper instructionWrapper)
        {
            var targetInstructions  = (Instruction[]) instructionWrapper.Instruction.Operand;
            foreach (var targetInstruction in targetInstructions)
            {
                TwoWayLinkExecutionPath(instructionWrapper,InstructionsWrappers.First(x => x.Instruction == targetInstruction));
            }
        }
    }
}
