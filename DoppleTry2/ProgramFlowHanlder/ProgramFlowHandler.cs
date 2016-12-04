using System.Collections.Generic;
using System.Linq;
using DoppleTry2.InstructionModifiers;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionWrappers;

namespace DoppleTry2.ProgramFlowHanlder
{
    abstract class ProgramFlowHandler
    {
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

        public abstract void SetForwardExecutionFlowInsts(InstructionWrapper wrapperToModify, List<InstructionWrapper> instructionWrappers);
    }
}
