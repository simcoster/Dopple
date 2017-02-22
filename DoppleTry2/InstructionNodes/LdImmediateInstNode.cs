using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace Dopple.InstructionNodes
{
    class LdImmediateInstNode : InstructionNode
    {
        public int ImmediateIntValue { get; private set; }
        public LdImmediateInstNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            ImmediateIntValue = GetImmediateInt(instruction);
        }

        private int GetImmediateInt(Instruction instruction)
        {
            var code = instruction.OpCode.Code;
            if (CodeGroups.LdImmediateValueCodes.Except(new[] { Code.Ldc_I4_M1 }).Contains(code))
                return int.Parse(code.ToString().Last().ToString());
            else if (CodeGroups.LdImmediateFromOperandCodes.Contains(code))
                return (Convert.ToInt32(instruction.Operand));
            else if (code == Code.Ldc_I4_M1)
                return -1;
            throw new Exception("No valid int value found");
        }
    }
}
