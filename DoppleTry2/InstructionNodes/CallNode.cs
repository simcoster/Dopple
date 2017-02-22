using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Dopple.InstructionNodes
{
    public abstract class CallNode : InstructionNode
    {
        public CallNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            TargetMethod = (MethodReference) instruction.Operand;
            StackPopCount = GetStackPopCount();
        }

        protected virtual int GetStackPopCount()
        {
            int stackPopCount = TargetMethod.Parameters.Count;
            if (TargetMethod.HasThis)
            {
                stackPopCount++;
            }
            return stackPopCount;
        }

        public MethodReference TargetMethod { get; private set; }
    }
}
