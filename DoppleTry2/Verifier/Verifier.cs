using System;
using System.Collections.Generic;
using DoppleTry2.InstructionNodes;
using Mono.Cecil.Cil;
using System.Linq;

namespace DoppleTry2.VerifierNs
{
    public abstract class Verifier
    {
        protected List<InstructionNode> instructionNodes;

        public Verifier(List<InstructionNode> instructionNodes)
        {
            this.instructionNodes = instructionNodes;
        }
        public abstract void Verify(InstructionNode instructionNode);

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
        public bool IsProvidingArray(InstructionNode insturctionWrapper)
        {
            if (insturctionWrapper.Instruction.OpCode.Code == Code.Newarr)
            {
                return true;
            }
            if (insturctionWrapper is LdArgInstructionNode &&
            ((LdArgInstructionNode)insturctionWrapper).ArgType.IsArray)
            {
                return true;
            }
            if (insturctionWrapper.Instruction.OpCode.StackBehaviourPush == StackBehaviour.Pushref)
            {
                return true;
            }
            if (CodeGroups.LdLocCodes.Contains(insturctionWrapper.Instruction.OpCode.Code))
            {
                return true;
            }
            return false;
        }
        public bool IsProvidingNumber(InstructionNode instructionWrapper)
        {
            if (instructionWrapper is LdImmediateInstNode)
            {
                return true;
            }
            if (instructionWrapper is LdArgInstructionNode && ((LdArgInstructionNode)instructionWrapper).ArgType.IsPrimitive)
            {
                return true;
            }
            if (new[] { StackBehaviour.Pushref, StackBehaviour.Push0 }.Contains(instructionWrapper.Instruction.OpCode.StackBehaviourPush))
            {
                return false;
            }
            return true;
        }

        public InstructionNode[] BacktraceStLdLoc (InstructionNode instructionWrapper)
        {
            if (CodeGroups.LdLocCodes.Concat(CodeGroups.StLocCodes).Concat(new[] { Code.Dup }).Contains(instructionWrapper.Instruction.OpCode.Code))
            {
                return instructionWrapper.DataFlowBackRelated.SelectMany(x => BacktraceStLdLoc(x.Argument)).ToArray();
            }
            else
            {
                return new[] { instructionWrapper };
            }
        }
    }

    public enum ValueType
    {
        Number,
        Array,
        Object,
        String,
        Null
    }
}