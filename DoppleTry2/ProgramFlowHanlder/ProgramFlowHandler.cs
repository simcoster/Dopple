using System.Collections.Generic;
using System.Linq;
using DoppleTry2.InstructionModifiers;
using Mono.Cecil.Cil;

namespace DoppleTry2.ProgramFlowHanlder
{
    abstract class ProgramFlowHandler
    {
        protected List<InstructionWrapper> InstructionWrappers;

        protected ProgramFlowHandler(List<InstructionWrapper> instructionWrappers)
        {
            InstructionWrappers = instructionWrappers;
        }

        public void TwoWayLinkExecutionPath(InstructionWrapper backInstruction, InstructionWrapper forwardInstruction)
        {
            backInstruction.NextPossibleProgramFlow.Add(forwardInstruction);
            forwardInstruction.BackProgramFlow.Add(backInstruction);
        }

        public abstract Code[] HandledCodes { get; }

        public void SetForwardExecutionFlowInsts(InstructionWrapper instructionWrapper)
        {
            SetForwardExecutionFlowInstsInternal(instructionWrapper);
        }

        protected abstract void SetForwardExecutionFlowInstsInternal(InstructionWrapper instructionWrapper);
    }
}
