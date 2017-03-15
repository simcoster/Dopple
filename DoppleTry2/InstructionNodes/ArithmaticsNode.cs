using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Runtime.Serialization;

namespace Dopple.InstructionNodes
{
    [DataContract]
    class ArithmaticsNode : InstructionNode
    {
        public ArithmaticsNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }
    }
}
