using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DoppleTry2.InstructionNodes
{
    class ConditionalJumpNode : InstructionNode
    {
        public ConditionalJumpNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }
    }
}
