using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.InstructionWrappers
{
    public class LdElemInstructionWrapper : InstructionWrapper
    {
        public LdElemInstructionWrapper(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            BackDataFlowRelated.MaxArgIndex = 2;
        }
    }
}
