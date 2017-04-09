using Dopple.BranchPropertiesNS;
using Dopple.Tracers.DynamicTracing;
using Dopple.Tracers.PredciateProviders;
using System.Collections.Generic;

namespace Dopple.BackTracers
{
    internal class MergeNodeTraceData
    {
        public BranchList ReachedBranches = new BranchList();
        public StateProviderCollection AccumelatedStateProviders = new StateProviderCollection();
        public bool AllBranchesReached = false;
    }
}