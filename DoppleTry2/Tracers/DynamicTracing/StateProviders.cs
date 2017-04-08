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
    class StateProviderCollection
    {
        private List<StoreDynamicDataStateProvider> _StateProviders = new List<StoreDynamicDataStateProvider>();
        public int Count
        {
            get
            {
                return _StateProviders.Count;
            }
        }
        public void Clear()
        {
            _StateProviders.Clear();
        }
        public void AddNewProvider(StoreDynamicDataStateProvider newStateProvider)
        {
            foreach (var overridedStore in _StateProviders.Where(x => newStateProvider.ShareNonObjectArgs(x)).ToList())
            {
                if (overridedStore.StoreNode == newStateProvider.StoreNode)
                {
                    _StateProviders.Remove(overridedStore);
                }
                else
                {
                    bool overidedIsInSameBranch = newStateProvider.StoreNode.BranchProperties.Branches.Except(overridedStore.StoreNode.BranchProperties.Branches).Any() == false;
                    if (overidedIsInSameBranch)
                    {
                        overridedStore.ObjectNodes = overridedStore.ObjectNodes.Except(newStateProvider.ObjectNodes).ToList();
                        if (overridedStore.ObjectNodes.Any() == false)
                        {
                            _StateProviders.Remove(overridedStore);
                        }
                    }

                }
            }
            _StateProviders.Add(newStateProvider);
        }

        public void AddNewProviders(StateProviderCollection stateProviders)
        {
            foreach (var stateProvider in stateProviders._StateProviders)
            {
                AddNewProvider(stateProvider);
            }
        }

        public void MergeBranches (InstructionNode mergeNode)
        {
            if (!mergeNode.BranchProperties.MergingNodeProperties.IsMergingNode)
            {
                throw new Exception("Node must be a branch merging node");
            }
            var GroupedProviders = new List<List<StoreDynamicDataStateProvider>>();
            foreach (var stateProvider in _StateProviders)
            {
                if (GroupedProviders.SelectMany(x => x).Contains(stateProvider))
                {
                    continue;
                }
                GroupedProviders.Add(_StateProviders.Where(x => stateProvider.ShareNonObjectArgs(x)).ToList());
            }
            foreach(var providerGroup in GroupedProviders)
            {
                if (mergeNode.BranchProperties.MergingNodeProperties.MergedBranches.All(x => providerGroup.Any(y => y.StoreNode.BranchProperties.Branches.Contains(x))))
                {
                    var overriddenProviders =  providerGroup.Where(x => x.StoreNode.BranchProperties.Branches.SequenceEqual(mergeNode.BranchProperties.Branches));
                    foreach (var overridenProvider in  overriddenProviders)
                    {
                        _StateProviders.Remove(overridenProvider);
                    }
                }
            }
        }

        public IEnumerable<InstructionNode> MatchLoadToStore(InstructionNode loadNode)
        {
            return _StateProviders.Where(x => x.IsLoadNodeMatching(loadNode)).Select(x => x.StoreNode);
        }

        internal StateProviderCollection Clone()
        {
            return new StateProviderCollection() { _StateProviders = new List<StoreDynamicDataStateProvider>(this._StateProviders) };
        }
    }
}
