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
            TargetMethodDefinition = (MethodDefinition)TargetMethod;
        }
        public InlineableCallNode(Instruction instruction,MethodDefinition calledFunc, MethodDefinition method) : base(instruction, method)
        {
            TargetMethodDefinition = calledFunc;
            SetStackPopCount(calledFunc);
            SetStackPushCount(calledFunc);
        }
       
        public MethodDefinition TargetMethodDefinition { get; private set; }
    }
}