using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace Dopple.InstructionNodes
{
    public class InlineableCallNode : CallNode
    {
        public InlineableCallNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            TargetMethodDefinion = (MethodDefinition)TargetMethod;
            SetStackPushCount(TargetMethodDefinion);
        }
        public InlineableCallNode(Instruction instruction,MethodDefinition calledFunc, MethodDefinition method) : base(instruction, method)
        {
            TargetMethodDefinion = calledFunc;
            SetStackPushCount(TargetMethodDefinion);
        }
        private void SetStackPushCount(MethodDefinition TargetMethod)
        {
            if (TargetMethod.ReturnType.FullName == "System.Void")
            {
                StackPushCount = 0;
            }
            else
            {
                StackPushCount = 1;
            }
        }
        public MethodDefinition TargetMethodDefinion { get; private set; }
    }
}