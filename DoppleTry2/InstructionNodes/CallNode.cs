using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DoppleTry2.InstructionNodes
{
    public abstract class CallNode : InstructionNode
    {
        public CallNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            TargetMethod = (MethodReference) instruction.Operand;
            StackPopCount = TargetMethod.Parameters.Count;
        }

        public MethodReference TargetMethod { get; private set; }
    }
}
