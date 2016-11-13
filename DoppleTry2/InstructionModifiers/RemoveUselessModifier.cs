using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionWrappers;


namespace DoppleTry2.InstructionModifiers
{
    class RemoveUselessModifier : IPreBacktraceModifier
    {
        public void Modify(List<InstructionWrapper> instructionWrappers)
        {
            var uselessCodes = new[] {Code.Nop, Code.Break};
         }
    }
}