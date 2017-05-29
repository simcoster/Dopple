using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;

namespace Dopple.Tracers.StateProviders.StoreToAddressStateProviders
{
    class StoreArgumentByAddressStateProvider : StoreArgumentStateProvider
    {
        public StoreArgumentByAddressStateProvider(InstructionNode storeInderectNode, InstructionNode loadAddressNode) : base(loadAddressNode)
        {
            _StoreInderectNode = storeInderectNode;
        }
        private InstructionNode _StoreInderectNode;

        public override void ConnectToLoadNode(InstructionNode loadNode)
        {
            var ldArgNode = (LdArgInstructionNode) loadNode;
            ldArgNode.DataFlowBackRelated.AddTwoWay(_StoreInderectNode, ldArgNode.DataFlowDataProdivderIndex);
        }
    }
}
