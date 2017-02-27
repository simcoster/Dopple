using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Dopple.InstructionNodes
{
    class SingleIndexDataTransferNode : DataTransferingNode
    {
        public SingleIndexDataTransferNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }

        public override int DataFlowDataProdivderIndex
        {
            get
            {
                return 0;
            }
        }
    }
}
