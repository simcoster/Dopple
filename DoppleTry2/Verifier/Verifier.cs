using System;
using System.Collections.Generic;
using DoppleTry2.InstructionWrappers;
using Mono.Cecil.Cil;

namespace DoppleTry2.VerifierNs
{
    public abstract class Verifier
    {
        public Verifier(List<InstructionWrapper> instructionWrappers)
        {
            _instructionWrappers = instructionWrappers;
        }
        protected List<InstructionWrapper> _instructionWrappers;
        public abstract void Verify(InstructionWrapper instructionWrapper);

        public static bool IsNumberType(Type value)
        {
            return value == typeof(sbyte)
                    || value == typeof(byte)
                    || value == typeof(short)
                    || value == typeof(ushort)
                    || value == typeof(int)
                    || value == typeof(uint)
                    || value == typeof(long)
                    || value == typeof(ulong)
                    || value == typeof(float)
                    || value == typeof(double)
                    || value == typeof(decimal);
        }
        public bool IsProvidingArray(InstructionWrapper insturctionWrapper)
        {
            if (insturctionWrapper.Instruction.OpCode.Code == Code.Newarr)
            {
                return true;
            }
            if (insturctionWrapper is LdArgInstructionWrapper &&
            ((LdArgInstructionWrapper)insturctionWrapper).ArgType.IsArray)
            {
                return true;
            }
            return false;
        }
        public bool IsProvidingNumber (InstructionWrapper instructionWrapper)
        {
            if (instructionWrapper is LdImmediateInstWrapper)
            {
                return true;
            }
            if (instructionWrapper is LdArgInstructionWrapper && ((LdArgInstructionWrapper)instructionWrapper).ArgType.IsPrimitive)
            {
                return true;
            }
            return false;
        }
    }
}