using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Dopple.VerifierNs;
using System.Runtime.Serialization;

namespace Dopple.InstructionNodes
{
    [DataContract]
    class StElemInstructionNode : InstructionNode, IDynamicDataStoreNode, IArrayUsingNode , IIndexUsingNode
    {
        public StElemInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }

        public IEnumerable<InstructionNode> ArrayArgs
        {
            get
            {
                return DataFlowBackRelated.Where(x => x.ArgIndex == 0).Select(x => x.Argument);
            }
        }

        public int DataFlowDataProdivderIndex
        {
            get
            {
                return 2;
            }
        }

        public IEnumerable<InstructionNode> IndexNodes
        {
            get
            {
                return DataFlowBackRelated.Where(x => x.ArgIndex == 1).Select(x => x.Argument);
            }
        }

        public int ObjectOrAddressArgIndex
        {
            get
            {
                return 0;
            }
        }       
    }
}
