using Dopple.Tracers.StateProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;

namespace Dopple.Tracers.StateProviders.StoreToAddressStateProviders
{
    class StoreElementByAddressStateProvider : StElemStateProvider
    {
        //Copy paste patern, not good, need to wrap a regular StoreElementProviderMaybe
        private InstructionNode _StoreInderectNode;

        public StoreElementByAddressStateProvider(InstructionNode storeInderectNode, InstructionNode loadAddressNode) : base(loadAddressNode)
        {
            _StoreInderectNode = storeInderectNode;
        }
        public override void ConnectToLoadNode(InstructionNode loadNode)
        {
            var loadElementNode = (LdElemInstructionNode) loadNode;
            loadElementNode.DataFlowBackRelated.AddTwoWay(_StoreInderectNode, loadElementNode.DataFlowDataProdivderIndex);
        }
    }
}
