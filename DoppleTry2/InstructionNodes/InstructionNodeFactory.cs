using DoppleTry2.InstructionNodes;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace DoppleTry2.InstructionNodes
{
    public class InstructionNodeFactory
    {
        SystemMethodsLoader systemMethodsLoader = new SystemMethodsLoader();
        public InstructionNode GetInstructionWrapper(Instruction instruction, MethodDefinition method)
        {
            Code nodeCode = instruction.OpCode.Code;
            if (CodeGroups.CallCodes.Contains(nodeCode))
            {
                MethodDefinition systemMethodDef = null;
                if (instruction.Operand is MethodDefinition)
                {
                    return new InlineableCallNode(instruction, method);
                }
                else if (systemMethodsLoader.TryGetSystemMethod(instruction, out systemMethodDef))
                {
                    return new InlineableCallNode(instruction, systemMethodDef,method);
                }
                else
                {
                    return new NonInlineableCallInstructionNode(instruction, method);
                }
            }
            else if (CodeGroups.LdArgCodes.Contains(nodeCode))
            {
                return new LdArgInstructionNode(instruction, method);
            }
            else if (CodeGroups.StArgCodes.Contains(nodeCode))
            {
                return new StArgInstructionNode(instruction, method);
            }
            else if (CodeGroups.LdLocCodes.Contains(nodeCode))
            {
                return new LocationLoadInstructionNode(instruction, method);
            }
            else if (CodeGroups.StLocCodes.Contains(nodeCode))
            {
                return new LocationStoreInstructionNode(instruction, method);
            }
            else if (CodeGroups.LdImmediateFromOperandCodes.Concat(CodeGroups.LdImmediateValueCodes).Contains(nodeCode))
            {
                return new LdImmediateInstNode(instruction, method);
            }
            else if (CodeGroups.LdElemCodes.Contains(nodeCode))
            {
                return new LdElemInstructionNode(instruction, method);
            }
            else if (nodeCode == Code.Ret)
            {
                return new RetInstructionNode(instruction, method);
            }
            else if (nodeCode == Code.Newobj)
            {
                return new NewObjInstructionNode(instruction, method);
            }
            else if (CodeGroups.CondJumpCodes.Contains(nodeCode))
            {
                return new ConditionalJumpNode(instruction, method);
            }
            else if (CodeGroups.StIndCodes.Contains(nodeCode))
            {
                return new StIndInstructionNode(instruction, method);
            }
            else
            {
                return new InstructionNode(instruction, method);
            }
        }

        private static bool TryGetSystemMethod(Instruction instruction, out MethodDefinition systemMethodDef)
        {
            throw new NotImplementedException();
        }
    }
}