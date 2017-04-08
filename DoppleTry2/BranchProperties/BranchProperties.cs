using Dopple.InstructionNodes;
using System;
using System.Collections.Generic;

namespace Dopple.BranchPropertiesNS
{
    public class BranchProperties
    {
        private static BaseBranch BaseBranch = new BaseBranch();
        public BranchList Branches { get; private set; } = new BranchList() { BaseBranch };
        public List<BranchID> CreatedBranches = new List<BranchID>();
        public MergingNodeProperties MergingNodeProperties { get; private set; } = new MergingNodeProperties();
        public List<InstructionNode> AffectedNodes = new List<InstructionNode>();
    }
}
