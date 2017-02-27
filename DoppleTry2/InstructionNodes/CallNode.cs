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
            SetStackPopCount(TargetMethod);
            SetStackPushCount(TargetMethod);
        }

        protected void SetStackPopCount(MethodReference targetMethod)
        {
            int stackPopCount = targetMethod.Parameters.Count;
            if (targetMethod.HasThis)
            {
                stackPopCount++;
            }
            StackPopCount =  stackPopCount;
            DataFlowBackRelated.ResetIndex();
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
