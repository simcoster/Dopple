using DoppleTry2.InstructionNodes;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace DoppleTry2.InstructionNodes
{
    public class InstructionNodeFactory
    {
        public static InstructionNode GetInstructionWrapper(Instruction instruction, MethodDefinition method)
        {
            if (CodeGroups.CallCodes.Contains(instruction.OpCode.Code))
            {
                if (instruction.Operand is MethodDefinition)
                {
                    return new InternalCallInstructionNode(instruction, method);
                }
                else
                {
                    return new ExternalCallInstructionNode(instruction, method);
                }
            }
            else if (CodeGroups.LdArgCodes.Contains(instruction.OpCode.Code))
            {
                return new LdArgInstructionNode(instruction, method);
            }
            else if (CodeGroups.StArgCodes.Contains(instruction.OpCode.Code))
            {
                return new StArgInstructionWrapper(instruction, method);
            }
            else if (CodeGroups.LdLocCodes.Contains(instruction.OpCode.Code))
            {
                return new LocationLoadInstructionNode(instruction, method);
            }
            else if (CodeGroups.StLocCodes.Contains(instruction.OpCode.Code))
            {
                return new LocationStoreInstructionNode(instruction, method);
            }
            else if (CodeGroups.LdImmediateFromOperandCodes.Concat(CodeGroups.LdImmediateValueCodes).Contains(instruction.OpCode.Code))
            {
                return new LdImmediateInstNode(instruction, method);
            }
            else if (CodeGroups.LdElemCodes.Contains(instruction.OpCode.Code))
            {
                return new LdElemInstructionNode(instruction, method);
            }
            else
            {
                return new InstructionNode(instruction, method);
            }
        }
    }
}