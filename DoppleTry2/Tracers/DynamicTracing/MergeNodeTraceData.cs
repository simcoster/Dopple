using Dopple.BranchPropertiesNS;
using Dopple.Tracers.DynamicTracing;
using Dopple.Tracers.PredciateProviders;
using System.Collections.Generic;

namespace Dopple.BackTracers
{
    internal class MergeNodeTraceData
    {
        public List<BranchID> ReachedBranches = new List<BranchID>();
        public StateProviders AccumelatedStateProviders = new StateProviders();
    }
}