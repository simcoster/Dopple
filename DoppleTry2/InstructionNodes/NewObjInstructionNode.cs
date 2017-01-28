using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DoppleTry2.InstructionNodes
{
    class NewObjInstructionNode : InstructionNode
    {
        public NewObjInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            MethodReference objCtor = (MethodReference) instruction.Operand;
            StackPopCount = objCtor.Parameters.Count;
        }
    }
}
