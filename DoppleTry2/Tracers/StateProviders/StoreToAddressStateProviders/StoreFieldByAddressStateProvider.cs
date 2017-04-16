using Dopple.Tracers.StateProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;

namespace Dopple.Tracers.StateProviders.StoreToAddressStateProviders
{
    class StoreFieldByAddressStateProvider : StoreFieldStateProvider
    {
        private InstructionNode _StoreInderectNode;

        public StoreFieldByAddressStateProvider(InstructionNode storeInderectNode, InstructionNode loadAddressNode) : base(loadAddressNode)
        {
            _StoreInderectNode = storeInderectNode;
        }
        public override void ConnectToLoadNode(InstructionNode loadNode)
        {
            var loadFieldNode = (LoadFieldNode) loadNode;
            loadFieldNode.DataFlowBackRelated.AddTwoWay(_StoreInderectNode, loadFieldNode.DataFlowDataProdivderIndex);
        }
    }
}
