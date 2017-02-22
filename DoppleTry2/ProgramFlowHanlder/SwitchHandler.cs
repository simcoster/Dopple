using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionModifiers;
using Mono.Cecil.Cil;
using Dopple.InstructionNodes;


namespace Dopple.ProgramFlowHanlder
{
    class SwitchHandler : ProgramFlowHandler
    {
        public override Code[] HandledCodes => new[] {Code.Switch};

        public override void SetForwardExecutionFlowInsts(InstructionNode wrapperToModify, List<InstructionNode> instructionWrappers)
        {
            var targetInstructions = (Instruction[])wrapperToModify.Instruction.Operand;
            foreach (var targetInstruction in targetInstructions)
            {
                instructionWrappers.First(x => x.Instruction == targetInstruction).ProgramFlowBackRoutes.AddTwoWay(wrapperToModify);
            }
        }
    }
}
