using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace DoppleTry2.InstructionNodes
{
    public class InlineableCallNode : CallNode
    {
        public InlineableCallNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            CalledFunction = (MethodDefinition)TargetMethod;
            SetStackPushCount(CalledFunction);
        }
        public InlineableCallNode(Instruction instruction,MethodDefinition calledFunc, MethodDefinition method) : base(instruction, method)
        {
            CalledFunction = calledFunc;
            SetStackPushCount(CalledFunction);
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
        public MethodDefinition CalledFunction { get; private set; }
    }
}