using Dopple.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.Tracers.ForwardTracers;

namespace Dopple.BranchPropertiesNS
{
    public class BranchID
    {
        private static object _LockGlobalIndex = new object();
        private static int GlobalIndex = 0;
        public BranchID(ConditionalJumpNode originatingNode)
        {
            lock(_LockGlobalIndex)
            {
                Index = GlobalIndex;
                GlobalIndex++;
            }
            OriginatingNode = originatingNode;
        }
        public int Index { get; private set; }
        public BranchType BranchType { get; set; }
        public ConditionalJumpNode OriginatingNode { get; set; }
        public PairedBranchIndex PairedBranchesIndex { get; set; } = PairedBranchIndex.First;
    }
}
