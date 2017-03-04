using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Dopple.VerifierNs;

namespace Dopple.InstructionNodes
{
    class StElemInstructionNode : InstructionNode, IDataTransferingNode
    {
        public StElemInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }

        public int DataFlowDataProdivderIndex
        {
            get
            {
                return 2;
            }
        }
    }
}
