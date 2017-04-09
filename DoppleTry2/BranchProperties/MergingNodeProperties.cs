using Dopple.InstructionNodes;
using System.Collections.Generic;

namespace Dopple.BranchPropertiesNS
{
    public class MergingNodeProperties
    {
        public MergingNodeProperties(InstructionNode node)
        {
            ContainingNodeForDebugging = node;
        }
        private InstructionNode ContainingNodeForDebugging;
        private bool _IsMergingNode;

        public bool IsMergingNode
        {
            get
            { return _IsMergingNode; }
            set
            {
                _IsMergingNode = value;
            }
        }
        public List<BranchID> MergedBranches { get; set; } = new List<BranchID>();
    }
}