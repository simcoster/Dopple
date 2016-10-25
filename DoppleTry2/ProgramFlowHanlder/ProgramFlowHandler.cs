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

        public List<InstructionWrapper> GetAllPreviousConnected(InstructionWrapper startInstruction, List<InstructionWrapper> visited = null)
        {
            if (visited == null)
            {
                visited = new List<InstructionWrapper>();
            }
            List<InstructionWrapper> prevInstructions = new List<InstructionWrapper>();
            if (startInstruction.BackProgramFlow.Count == 0)
            {
                return prevInstructions;
            }
            if (CodeGroups.CallCodes.Concat(new[] { Code.Ret }).Contains( startInstruction.Instruction.OpCode.Code ))
            {
                return prevInstructions;
            }
            if (visited.Contains(startInstruction))
            {
                return prevInstructions;
            }
            visited.Add(startInstruction);

            var recursivePrevConnected = startInstruction.BackProgramFlow.SelectMany(x => GetAllPreviousConnected(x, visited));
            prevInstructions.AddRange(recursivePrevConnected);
            prevInstructions.Add(startInstruction);
            return prevInstructions;
        }

        public abstract Code[] HandledCodes { get; }

        public void SetForwardExecutionFlowInsts(InstructionWrapper instructionWrapper)
        {
            SetForwardExecutionFlowInstsInternal(instructionWrapper);
        }

        protected abstract void SetForwardExecutionFlowInstsInternal(InstructionWrapper instructionWrapper);
    }
}
