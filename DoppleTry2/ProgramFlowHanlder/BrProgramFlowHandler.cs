using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using Dopple.InstructionNodes;


namespace Dopple.ProgramFlowHanlder
{
    class BrProgramFlowHandler : ProgramFlowHandler
    {
        public override Code[] HandledCodes => new[] {Code.Br_S, Code.Br};

        public override void SetForwardExecutionFlowInsts(InstructionNode wrapperToModify, List<InstructionNode> instructionWrappers)
        {
            InstructionNode nextInstruction =
             instructionWrappers.First(x => x.Instruction == wrapperToModify.Instruction.Operand);
            nextInstruction.ProgramFlowBackRoutes.AddTwoWay(wrapperToModify);
        }
    }
}