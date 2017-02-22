using System;
using System.Collections.Generic;
using System.Linq;
using Dopple.InstructionModifiers;
using Mono.Cecil.Cil;
using Dopple.InstructionNodes;

namespace Dopple.ProgramFlowHanlder
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

        private readonly Code[] _unhandledCodes = new[] { Code.Br, Code.Br_S, Code.Ret };

        public override Code[] HandledCodes { get; }

        public override void SetForwardExecutionFlowInsts(InstructionNode wrapperToModify, List<InstructionNode> instructionWrappers)
        {
            InstructionNode nextInstructionWrapper =
               instructionWrappers.FirstOrDefault(x => x.Instruction == wrapperToModify.Instruction.Next);
            if (nextInstructionWrapper == null)
            {
                return;
            }
            nextInstructionWrapper.ProgramFlowBackRoutes.AddTwoWay(wrapperToModify);
        }
    }
}