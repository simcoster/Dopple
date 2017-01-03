using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace DoppleTry2.InstructionNodes
{
    public class InlineableCallNode : InstructionNode
    {
        public InlineableCallNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            CalledFunction = (MethodDefinition)Instruction.Operand;
            StackPopCount = CalledFunction.Parameters.Count;
        }
        public MethodDefinition CalledFunction { get; private set; }
    }
}