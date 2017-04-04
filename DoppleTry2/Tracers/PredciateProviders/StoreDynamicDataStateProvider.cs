using Dopple.BranchPropertiesNS;
using Dopple.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dopple.Tracers.PredciateProviders
{
    abstract class StoreDynamicDataStateProvider
    {
        internal StoreDynamicDataStateProvider(InstructionNode storeNode)
        {
            StoreNode = storeNode;
            ObjectNodes = GetObjectArgs(storeNode);
        }

        internal abstract List<InstructionNode> GetObjectArgs(InstructionNode storeNode);
        public abstract bool IsLoadNodeMatching(InstructionNode loadNode);
        public InstructionNode StoreNode { get; private set; }
        public List<InstructionNode> ObjectNodes { get; set; }
        public abstract bool ShareNonObjectArgs(StoreDynamicDataStateProvider newStore);
        protected bool ShareObejctArgs(InstructionNode loadNode)
        {
            var loadNodeNodeObjects = loadNode.DataFlowBackRelated.Where(x => x.ArgIndex == 0).SelectMany(x => x.Argument.GetDataOriginNodes());
            return ObjectNodes.Intersect(loadNodeNodeObjects).Any();
        }

        public static StoreDynamicDataStateProvider GetMatchingStateProvider(InstructionNode storeNode)
        {
            if (storeNode is StElemInstructionNode)
            {
                return new StElemStateProvider(storeNode);
            }
            if (storeNode is StoreFieldNode)
            {
                return new StoreFieldStateProvider(storeNode);
            }
            return null;
        }

    }
}
