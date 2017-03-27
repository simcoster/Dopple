using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.InstructionNodes
{
    [DataContract]
    public class LdElemInstructionNode : ObjectOrAddressRequiringNode, IArrayUsingNode, IIndexUsingNode, IDynamicDataLoadNode
    {
        public LdElemInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            DataFlowBackRelated.MaxArgIndex = 2;
        }
        public bool AllPathsHaveAStoreNode { get; set; } = false;


        public IEnumerable<InstructionNode> ArrayBackArgs
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
    }
}
