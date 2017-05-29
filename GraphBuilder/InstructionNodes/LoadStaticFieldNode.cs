using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Dopple.InstructionNodes
{
    internal class LoadStaticFieldNode : FieldManipulationNode , IDynamicDataLoadNode
    {
        public LoadStaticFieldNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }

        public bool AllPathsHaveAStoreNode { get; set; } = false;


        public int DataFlowDataProdivderIndex
        {
            get
            {
                return 0;
            }
        }
    }
}