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
        public override Code[] HandledCodes => new[] {Code.Switch};

        protected override void SetForwardExecutionFlowInstsInternal(InstructionWrapper wrapperToModify, List<InstructionWrapper> instructionWrappers)
        {
            var targetInstructions = (Instruction[])wrapperToModify.Instruction.Operand;
            foreach (var targetInstruction in targetInstructions)
            {
                TwoWayLinkExecutionPath(wrapperToModify, instructionWrappers.First(x => x.Instruction == targetInstruction));
            }
        }
    }
}
