using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionNodes;


namespace DoppleTry2.InstructionModifiers
{
    class RemoveUselessModifier : IPreBacktraceModifier
    {
        public void Modify(List<InstructionNode> instructionWrappers)
        {
            var uselessCodes = new[] {Code.Nop, Code.Break};
         }
    }
}