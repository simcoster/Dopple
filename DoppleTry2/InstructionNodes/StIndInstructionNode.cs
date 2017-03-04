using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Dopple.InstructionNodes
{
    public class StIndInstructionNode : InstructionNode ,IDataTransferingNode
    {
        public List<InstructionNode> AddressProvidingArgs = new List<InstructionNode>();
        public StIndInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }
        internal override void SelfRemoveAndStitch()
        {
            DataFlowBackRelated.RemoveAllTwoWay(x => x.ArgIndex == 0);
            base.SelfRemoveAndStitch();
        }

        public AddressType AddressType { get; set; }

        public int DataFlowDataProdivderIndex
        {
            get
            {
                return 0;
            }
        }
    }

    public enum AddressType
    {
        LocalVar,
        Argument,
        ArrayElem,
        Field,
        GeneralData
    }
}
