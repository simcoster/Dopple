using Dopple.InstructionNodes;
using System.Collections.Generic;
using System;

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
        public void AddTwoWay(InstructionNode node)
        {
            node.BranchProperties.Branches.Add(this);
            BranchNodes.Add(node);
        }
        internal void AddTwoWay(List<InstructionNode> instructionNodes)
        {
            foreach (var node in instructionNodes)
            {
                AddTwoWay(node);
            }
        }
        public void RemoveTwoWay(InstructionNode node)
        {
            node.BranchProperties.Branches.Remove(this);
            BranchNodes.Remove(node);
        }
        public int Index { get; protected set; }
        public virtual BranchType BranchType { get; set; }
        public InstructionNode OriginatingNode { get; set; }
        public int PairedBranchesIndex { get; set; }
        public List<InstructionNode> BranchNodes { get { return _BranchNodes; } } 
        public InstructionNode MergingNode { get; set; }
        
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
