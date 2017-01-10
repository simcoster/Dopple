using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DoppleTry2.InstructionNodes
{
    internal class NonInlineableCallInstructionNode : CallNode
    {
        public NonInlineableCallInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
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
    }
}