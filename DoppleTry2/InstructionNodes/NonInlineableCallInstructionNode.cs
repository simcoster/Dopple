using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Runtime.Serialization;

namespace Dopple.InstructionNodes
{
    [DataContract]
    public class NonInlineableCallInstructionNode : CallNode
    {
        public NonInlineableCallInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }
    }
}