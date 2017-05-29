using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Dopple.InstructionNodes
{
    class LdindNode : InstructionNode, IDynamicDataLoadNode
    {
        public LdindNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }

        public bool AllPathsHaveAStoreNode { get; set; }

        public int DataFlowDataProdivderIndex
        {
            get
            {
                return 1;
            }
        }
    }
}
