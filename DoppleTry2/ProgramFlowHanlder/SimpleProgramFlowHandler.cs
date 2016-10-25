using System.Collections.Generic;
using System.Linq;
using DoppleTry2.InstructionModifiers;
using Mono.Cecil.Cil;

namespace DoppleTry2.ProgramFlowHanlder
{
    class SimpleProgramFlowHandler : ProgramFlowHandler
    {
        public SimpleProgramFlowHandler(List<InstructionWrapper> instructionWrappers) : base(instructionWrappers)
        {
            HandledCodes = typeof(OpCodes).GetFields()
                .Select(x => x.GetValue(null))
                .Cast<OpCode>()
                .Select(y => y.Code).
                Except(_unhandledCodes).ToArray();
        }

        private readonly Code[] _unhandledCodes = new[] { Code.Br, Code.Br_S }.Concat(CodeGroups.CallCodes).ToArray();

        public override Code[] HandledCodes { get; }

        protected override void SetForwardExecutionFlowInstsInternal(InstructionWrapper instructionWrapper)
        {
            InstructionWrapper nextInstructionWrapper =
                InstructionWrappers.FirstOrDefault(x => x.Instruction == instructionWrapper.Instruction.Next);
            if (nextInstructionWrapper == null)
            {
                return;
            }
            TwoWayLinkExecutionPath(instructionWrapper, nextInstructionWrapper);
        }
    }
}