using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace DoppleTry2.InstructionWrappers
{
    public class LdArgInstructionWrapper : FunctionArgInstWrapper
    {
        public LdArgInstructionWrapper(Instruction instruction, MethodDefinition method) : base(instruction, method) { }
    }

    public class StArgInstructionWrapper : FunctionArgInstWrapper
    {
        public StArgInstructionWrapper(Instruction instruction, MethodDefinition method) : base(instruction, method) { }
    }

    public abstract class FunctionArgInstWrapper : InstructionWrapper
    {
        public FunctionArgInstWrapper(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            ArgIndex = GetArgIndex(instruction);
            if (!method.IsStatic && instruction.Operand==null)
            {
                ArgIndex--;
            }
            ArgName = method.Parameters[ArgIndex].Name;
            ArgType = method.Parameters[ArgIndex].ParameterType;
        }
        public int ArgIndex { get; set; }
        public TypeReference ArgType { get; set; }
        public string ArgName { get; set; }
        private int GetArgIndex(Instruction instruction)
        {
            switch (instruction.OpCode.Code)
            {
                case Code.Ldarg_0:
                    return 0;
                case Code.Ldarg_1:
                    return 1;
                case Code.Ldarg_2:
                    return 2;
                case Code.Ldarg_3:
                    return 3;
            }
            if (instruction.Operand is ValueType)
                return Convert.ToInt32(instruction.Operand);
            else
                if (instruction.Operand is VariableDefinition)
                return ((VariableDefinition)instruction.Operand).Index - 1;
            else if (instruction.Operand is ParameterDefinition)
                return ((ParameterDefinition)instruction.Operand).Index;
            throw new Exception("shouldn't get here");
        }
    }
}