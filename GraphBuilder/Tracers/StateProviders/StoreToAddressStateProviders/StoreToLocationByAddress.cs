using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;

namespace Dopple.Tracers.StateProviders.StoreToAddressStateProviders
{
    class StoreToLocationByAddress : StoreLocationStateProvider
    {
        private InstructionNode _StoreInderectNode;

        public StoreToLocationByAddress(InstructionNode storeInderectNode, InstructionNode loadAddressNode) : base(loadAddressNode)
        {
            _StoreInderectNode = storeInderectNode;
        }
        public override void ConnectToLoadNode(InstructionNode loadNode)
        {
            var loadLocNode = (LocationLoadInstructionNode) loadNode;
            loadLocNode.DataFlowBackRelated.AddTwoWay(_StoreInderectNode, loadLocNode.DataFlowDataProdivderIndex);
        }
    }
}
