using Dopple.BranchPropertiesNS;
using Dopple.InstructionNodes;
using Dopple.Tracers.PredciateProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.Tracers.DynamicTracing
{
    class StateProviders
    {
        private List<StoreDynamicDataStateProvider> storePredicateProviders = new List<StoreDynamicDataStateProvider>();
        public void AddNewProvider(StoreDynamicDataStateProvider storePredicateProvider)
        {
            foreach (var overridedStore in storePredicateProviders.Where(x => storePredicateProvider.ShareNonObjectArgs(x)).ToList())
            {
                bool overidedIsInSubranch = !storePredicateProvider.StoreNode.BranchProperties.Branches.Except(overridedStore.StoreNode.BranchProperties.Branches).Any();
                if (overidedIsInSubranch)
                {
                    storePredicateProviders.Remove(overridedStore);
                }
                else if (overridedStore.ObjectNodes.Intersect(storePredicateProvider.ObjectNodes).Any())
                {
                    overridedStore.ObjectNodes = overridedStore.ObjectNodes.Except(storePredicateProvider.ObjectNodes).ToList();
                }
                storePredicateProviders.Add(storePredicateProvider);
            }
        }

        public void MergeBranches (InstructionNode mergeNode)
        {
            if (!mergeNode.BranchProperties.MergingNodeProperties.IsMergingNode)
            {
                throw new Exception("Node must be a branch merging node");
            }
            var GroupedProviders = new List<List<StoreDynamicDataStateProvider>>();
            foreach (var predicateProvider in storePredicateProviders)
            {
                if (GroupedProviders.SelectMany(x => x).Contains(predicateProvider))
                {
                    continue;
                }
                GroupedProviders.Add(storePredicateProviders.Where(x => predicateProvider.ShareNonObjectArgs(x)).ToList());
            }
            foreach(var providerGroup in GroupedProviders)
            {
                if (mergeNode.BranchProperties.MergingNodeProperties.MergedBranches.All(x => providerGroup.Any(y => y.StoreNode.BranchProperties.Branches.Contains(x))))
                {
                    providerGroup.RemoveAll(x => x.StoreNode.BranchProperties.Branches.SequenceEqual(mergeNode.BranchProperties.Branches));
                }
            }
        }

        public IEnumerable<InstructionNode> MatchLoadToStore(InstructionNode loadNode)
        {
            return storePredicateProviders.Where(x => x.IsLoadNodeMatching(loadNode)).Select(x => x.StoreNode);
        }
    }
}
