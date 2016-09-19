using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionModifiers;
using Mono.Cecil.Cil;

namespace DoppleTry2.ProgramFlowHanlder
{
    class SwitchHandler : ProgramFlowHandler
    {
        public SwitchHandler(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        public override Code[] HandledCodes => new[] {Code.Switch};
        public override void SetForwardExecutionFlowInsts(InstructionWrapper instructionWrapper)
        {
            var targetInstructions  = (Instruction[]) instructionWrapper.Instruction.Operand;
            foreach (var targetInstruction in targetInstructions)
            {
                TwoWayLinkExecutionPath(instructionWrapper, InstructionWrappers.First(x => x.Instruction == targetInstruction));
            }
        }
    }
}
