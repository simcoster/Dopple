using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace Dopple.InstructionNodes
{
    internal class LoadFieldNode : InstructionNode, IDataTransferingNode
    {
        public FieldDefinition FieldDefinition;
        public LoadFieldNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            if (instruction.Operand is FieldDefinition)
            {
                FieldDefinition = (FieldDefinition) instruction.Operand;
            }
            else if (instruction.Operand is FieldReference)
            {
                FieldDefinition = ((FieldReference) instruction.Operand).Resolve();
            }
            else
            {
                throw new Exception("Operand must be a field reference");
            }
        }

        public int DataFlowDataProdivderIndex
        {
            get
            {
                return 1;
            }
        }
    }
}