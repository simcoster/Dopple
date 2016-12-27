using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace DoppleTry2.InstructionNodes
{
    public class CallInstructionWrapper : InstructionNode
    {
        public CallInstructionWrapper(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            CalledFunction = (MethodDefinition)Instruction.Operand;
        }
        public MethodDefinition CalledFunction { get; private set; }
    }
}