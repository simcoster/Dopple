using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace DoppleTry2.InstructionModifiers
{
    class InlineCalledMethods : IModifier
    {
        private static Code[] _relevantCodes = {Code.Call, Code.Calli, Code.Callvirt};
        public void Modify(List<InstructionWrapper> instructionWrappers)
        {
            foreach (var instructionWrapper in instructionWrappers.Where(x => _relevantCodes.Contains(x.Instruction.OpCode.Code)))
            {
                if (instructionWrappers)
            }
        }
    }
}
