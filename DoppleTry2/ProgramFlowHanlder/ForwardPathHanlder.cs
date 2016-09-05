using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace DoppleTry2.InstructionModifiers
{
    abstract class ForwardPathHanlder 
    {
        protected readonly List<InstructionWrapper> InstructionsWrappers;

        protected ForwardPathHanlder(List<InstructionWrapper> instructionsWrappers)
        {
            InstructionsWrappers = instructionsWrappers;
        }

        protected void TwoWayLinkExecutionPath(InstructionWrapper backInstruction, InstructionWrapper forwardInstruction)
        {
            backInstruction.Next.Add(forwardInstruction);
            forwardInstruction.Back.Add(backInstruction);
        }

        protected abstract Code[] HandledCodes { get; }

        protected abstract void SetForwardExecutionFlowInsts(InstructionWrapper instructionWrapper);
    }
}
