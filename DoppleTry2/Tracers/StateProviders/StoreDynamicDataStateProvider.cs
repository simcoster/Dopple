using Dopple.BranchPropertiesNS;
using Dopple.InstructionNodes;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dopple.Tracers.StateProviders
{
    abstract class StoreDynamicDataStateProvider
    {
        internal StoreDynamicDataStateProvider(InstructionNode storeNode)
        {
            StoreNode = storeNode;
        }
        public abstract bool IsLoadNodeMatching(InstructionNode loadNode);
        public InstructionNode StoreNode { get; private set; }
        public Guid MyGuid { get; set; } = Guid.NewGuid();
        public static IEnumerable<StoreDynamicDataStateProvider> GetMatchingStateProvider(InstructionNode storeNode)
        {
            StoreDynamicDataStateProvider singleStateProvider = GetSingleMatchingStateProvider(storeNode);
            if (singleStateProvider != null)
            {
                return new[] { singleStateProvider };
            }
            if (CodeGroups.StIndCodes.Contains(storeNode.Instruction.OpCode.Code))
            {
                return StoreToAddressFactory.GetStoreToAddressStateProvider(storeNode);
            }
            return null;
        }

        private static StoreDynamicDataStateProvider GetSingleMatchingStateProvider (InstructionNode storeNode)
        {
            if (storeNode is StElemInstructionNode)
            {
                return new StElemStateProvider(storeNode);
            }
            if (storeNode is StoreFieldNode)
            {
                return new StoreFieldStateProvider(storeNode);
            }
            if (storeNode is StoreStaticFieldNode)
            {
                return new StoreStaticFieldStateProvider(storeNode);
            }
            if (storeNode is LocationStoreInstructionNode)
            {
                return new StoreLocationStateProvider(storeNode);
            }
            return null;
        }
        internal void OverrideAnother(StoreDynamicDataStateProvider partiallyOverrided, out bool completelyOverrides)
        {
            if (partiallyOverrided.GetType() != this.GetType())
            {
                completelyOverrides = false;
                return;
            }
            if (partiallyOverrided.StoreNode == StoreNode)
            {
                completelyOverrides = true;
                return;
            }
            OverrideAnotherInternal(partiallyOverrided, out completelyOverrides);
        }

        public virtual void ConnectToLoadNode(InstructionNode loadNode)
        {
            loadNode.DataFlowBackRelated.AddTwoWay(StoreNode, ((IDynamicDataLoadNode) loadNode).DataFlowDataProdivderIndex);
        }
        protected abstract void OverrideAnotherInternal(StoreDynamicDataStateProvider overrideCandidate, out bool completelyOverrides);

    }


  
}
