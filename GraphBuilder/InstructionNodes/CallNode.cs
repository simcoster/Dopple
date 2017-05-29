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
        internal VirtualCallInstructionNode OriginalVirtualNode { get; set; }
        public CallNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            TargetMethod = (MethodReference) instruction.Operand;
            SetStackPopCount(TargetMethod);
            SetStackPushCount(TargetMethod);
        }

        protected void SetStackPopCount(MethodReference targetMethod)
        {
            int tempStackPopCount = targetMethod.Parameters.Count;
            if (targetMethod.HasThis)
            {
                tempStackPopCount++;
            }
            StackPopCount = tempStackPopCount;
        }

        protected void SetStackPushCount(MethodReference targetMethod)
        {
            if (targetMethod.ReturnType.FullName == "System.Void")
            {
                StackPushCount =  0;
            }
            else
            {
                StackPushCount = 1;
            }
        }
        public MethodReference TargetMethod { get; private set; }
    }
}
