using Dopple.BranchPropertiesNS;
using Dopple.InstructionNodes;
using Dopple.Tracers.DynamicTracing;
using Dopple.Tracers.StateProviders;
using System.Collections.Generic;

namespace Dopple.BackTracers
{
    internal class MergeNodeTraceData
    {
        public List<InstructionNode> ReachedNodes = new List<InstructionNode>();
        public List<StoreDynamicDataStateProvider> AccumelatedStateProviders = new List<StoreDynamicDataStateProvider>();
    }
}