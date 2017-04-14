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

        public StateProviderCollection(List<StoreDynamicDataStateProvider> stateProviders)
        {
            _StateProviders = stateProviders.GroupBy(x => x.StoreNode).Select(x => x.First()).ToList();
        }

        public StateProviderCollection()
        {
        }

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
            foreach (var overridedStore in _StateProviders.ToList())
            {
                bool completelyOverrides;
                newStateProvider.OverrideAnother(overridedStore, out completelyOverrides);
                if (completelyOverrides)
                {
                    _StateProviders.Remove(overridedStore);
                }
            }
        }

        public void AddNewProviders(IEnumerable<StoreDynamicDataStateProvider> stateProviders)
        {
            foreach (var stateProvider in stateProviders)
            {
                AddNewProvider(stateProvider);
            }
        }      

        public IEnumerable<InstructionNode> MatchLoadToStore(InstructionNode loadNode)
        {
            return _StateProviders.Where(x => x.IsLoadNodeMatching(loadNode)).Select(x => x.StoreNode);
        }

        internal StateProviderCollection Clone()
        {
            return new StateProviderCollection(this._StateProviders);
        }
        
        public List<StoreDynamicDataStateProvider> ToList()
        {
            return _StateProviders;
        }
    }
}
