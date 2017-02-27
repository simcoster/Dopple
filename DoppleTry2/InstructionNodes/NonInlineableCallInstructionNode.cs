using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Dopple.InstructionNodes
{
    internal class NonInlineableCallInstructionNode : CallNode
    {
        public NonInlineableCallInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }
    }
}