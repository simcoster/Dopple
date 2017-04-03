using Dopple.BranchPropertiesNS;
using Dopple.InstructionNodes;
using System.Collections.Generic;

namespace Dopple.Tracers.PredciateProviders
{
    abstract class StoreDynamicDataPredicateProvider
    {
        public StoreDynamicDataPredicateProvider(InstructionNode storeNode, BranchID originatingBranch)
        {
            StoreNode = storeNode;
        }
        public abstract PredicateAndNode GetMatchingLoadPredicate(InstructionNode storeNode);
        public abstract bool IsRelevant(InstructionNode storeNode);
        public InstructionNode StoreNode { get; private set; }
        public List<InstructionNode> ObjectNodes = new List<InstructionNode>();
        public abstract bool IsOverriding(StoreDynamicDataPredicateProvider newStore);
        public BranchID OriginatingBranch { get; set; }
    }
}
