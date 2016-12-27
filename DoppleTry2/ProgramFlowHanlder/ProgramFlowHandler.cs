using System.Collections.Generic;
using System.Linq;
using DoppleTry2.InstructionModifiers;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionNodes;

namespace DoppleTry2.ProgramFlowHanlder
{
    abstract class ProgramFlowHandler
    {
        public List<InstructionNode> GetAllPreviousConnected(InstructionNode startInstruction, List<InstructionNode> visited = null)
        {
            if (visited == null)
            {
                visited = new List<InstructionNode>();
            }
            List<InstructionNode> prevInstructions = new List<InstructionNode>();
            if (startInstruction.ProgramFlowBackRoutes.Count == 0)
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

            var recursivePrevConnected = startInstruction.ProgramFlowBackRoutes.SelectMany(x => GetAllPreviousConnected(x, visited));
            prevInstructions.AddRange(recursivePrevConnected);
            prevInstructions.Add(startInstruction);
            return prevInstructions;
        }

        public abstract Code[] HandledCodes { get; }

        public abstract void SetForwardExecutionFlowInsts(InstructionNode wrapperToModify, List<InstructionNode> instructionWrappers);
    }
}
