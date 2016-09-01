using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;

namespace DoppleTry2.InstructionModifiers
{
    class RemoveUselessModifier : IModifier
    {
        public void Modify(List<InstructionWrapper> instructionWrappers)
        {
            var uselessCodes = new[] {Code.Nop, Code.Break};
            instructionWrappers.RemoveAll(x => uselessCodes.Contains(x.Instruction.OpCode.Code));
        }
    }
}