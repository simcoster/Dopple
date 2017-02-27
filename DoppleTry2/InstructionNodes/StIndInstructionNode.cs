using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Dopple.InstructionNodes
{
    public class StIndInstructionNode : DataTransferingNode
    {
        public List<InstructionNode> AddressProvidingArgs = new List<InstructionNode>();
        public StIndInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }
        internal override void SelfRemove()
        {
            DataFlowBackRelated.RemoveAllTwoWay(x => x.ArgIndex == 0);
            base.SelfRemove();
        }

        public AddressType AddressType { get; set; }

        public override int DataFlowDataProdivderIndex
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
