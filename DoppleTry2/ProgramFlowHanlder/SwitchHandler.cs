using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionModifiers;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionWrappers;


namespace DoppleTry2.ProgramFlowHanlder
{
    class SwitchHandler : ProgramFlowHandler
    {
        public override Code[] HandledCodes => new[] {Code.Switch};

        public override void SetForwardExecutionFlowInsts(InstructionWrapper wrapperToModify, List<InstructionWrapper> instructionWrappers)
        {
            var targetInstructions = (Instruction[])wrapperToModify.Instruction.Operand;
            foreach (var targetInstruction in targetInstructions)
            {
                instructionWrappers.First(x => x.Instruction == targetInstruction).BackProgramFlow.AddTwoWay(wrapperToModify);
            }
        }
    }
}
