using System.Collections.Generic;

namespace Dopple.BranchPropertiesNS
{
    public class MergingNodeProperties
    {
        public bool IsMergingNode { get; set; }
        public List<BranchID> MergedBranches { get; set; } = new List<BranchID>();
    }
}