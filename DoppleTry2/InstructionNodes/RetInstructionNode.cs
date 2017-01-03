using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DoppleTry2.InstructionNodes
{
    class RetInstructionNode : InstructionNode
    {
        public RetInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            if (Method.ReturnType.FullName == "System.Void")
            {
                StackPushCount = 0;
            }
            else
            {
                StackPopCount = 1;
                StackPushCount = 1;
            }
        }
    }
}
