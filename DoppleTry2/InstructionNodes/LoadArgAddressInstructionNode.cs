using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Dopple.InstructionNodes
{
    class LoadArgAddressInstructionNode : LdArgInstructionNode
    {
        public LoadArgAddressInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }
    }
}
