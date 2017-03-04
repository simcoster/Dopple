using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Dopple.InstructionNodes
{
    class SingleIndexDataTransferNode : InstructionNode, IDataTransferingNode
    {
        public SingleIndexDataTransferNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }

        public int DataFlowDataProdivderIndex
        {
            get
            {
                return 0;
            }
        }
    }
}
