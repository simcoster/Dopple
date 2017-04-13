using Dopple.InstructionNodes;
using System;
using System.Collections.Generic;

namespace Dopple.BranchPropertiesNS
{
    public class BranchProperties
    {
        public BranchProperties(InstructionNode node)
        {
            ContainingNodeForDebugging = node;
            MergingNodeProperties = new MergingNodeProperties(node);
        }
        private InstructionNode ContainingNodeForDebugging;
        public static BaseBranch BaseBranch = new BaseBranch();
        public BranchList Branches { get; private set; } = new BranchList() { BaseBranch };
        public MergingNodeProperties MergingNodeProperties { get; private set; }
        public bool FirstInLoop { get; internal set; }
    }
}
