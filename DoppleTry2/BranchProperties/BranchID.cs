using Dopple.InstructionNodes;
using System.Collections.Generic;

namespace Dopple.BranchPropertiesNS
{
    public class BranchID
    {
        public static List<BranchID> AllBranches = new List<BranchID>();
        private static object _LockGlobalIndex = new object();
        private static int GlobalIndex = 0;
        private List<InstructionNode> _BranchNodes = new List<InstructionNode>();

        public BranchID(InstructionNode originatingNode)
        {
            lock(_LockGlobalIndex)
            {
                Index = GlobalIndex;
                GlobalIndex++;
            }
            OriginatingNode = originatingNode;
            //TODO not got design, parent should not be aware of sons
            if (!(this is BaseBranch))
            {
                AllBranches.Add(this);
            }
            BranchType = BranchType.Exit;
        }
        public int Index { get; protected set; }
        public virtual BranchType BranchType { get; set; }
        public InstructionNode OriginatingNode { get; set; }
        public int PairedBranchesIndex { get; set; }
        public List<InstructionNode> BranchNodes { get { return _BranchNodes; } } 
        public InstructionNode MergingNode { get; set; }

        public static void Reset()
        {
            foreach(var branch in AllBranches)
            {
                branch.BranchNodes.ForEach(x => { x.BranchProperties.Branches.Remove(branch); });
                if (branch.MergingNode != null)
                {
                    branch.MergingNode.BranchProperties.MergingNodeProperties.IsMergingNode = false;
                    branch.MergingNode.BranchProperties.MergingNodeProperties.MergedBranches.Clear();
                }
                ((ConditionalJumpNode) branch.OriginatingNode).CreatedBranches.Clear();
                branch.BranchNodes.Clear();
            }
            AllBranches.Clear();
            GlobalIndex = 1;
        }
    }

    public class BaseBranch : BranchID
    {
        public BaseBranch() : base(null)
        {
        }

        public override BranchType BranchType
        {
            get
            {
                return BranchType.Exit;
            }
            set
            {
            }
        }
    }
}
