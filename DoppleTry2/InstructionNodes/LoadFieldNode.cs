using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace Dopple.InstructionNodes
{
    internal abstract class FieldManipulationNode : ObjectOrAddressRequiringNode 
    {
        public FieldDefinition FieldDefinition;
        public FieldManipulationNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
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
    }

    internal class LoadFieldNode : FieldManipulationNode, IDynamicDataLoadNode
    {
        public LoadFieldNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }
        public bool AllPathsHaveAStoreNode { get; set; } = false;

        public int DataFlowDataProdivderIndex
        {
            get
            {
                return 1;
            }
        }
    }

    internal class StoreFieldNode : FieldManipulationNode, IDynamicDataStoreNode
    {
        public StoreFieldNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
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