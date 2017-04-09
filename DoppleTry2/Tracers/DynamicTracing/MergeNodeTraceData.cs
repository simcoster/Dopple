using Dopple.BranchPropertiesNS;
using Dopple.InstructionNodes;
using Dopple.Tracers.DynamicTracing;
using Dopple.Tracers.PredciateProviders;
using System.Collections.Generic;

namespace Dopple.BackTracers
{
    internal class MergeNodeTraceData
    {
        public List<InstructionNode> ReachedNodes = new List<InstructionNode>();
        public StateProviderCollection AccumelatedStateProviders = new StateProviderCollection();
        public bool AllBranchesReached = false;
    }
}