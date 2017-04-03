using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.BranchPropertiesNS
{
    public class BranchProperties
    {
        private static BranchID BaseBranch = new BranchID(null);
        public List<BranchID> Branches { get; private set; } = new List<BranchID>() { BaseBranch };
        public MergingNodeProperties MergingNodeProperties { get; private set; } = new MergingNodeProperties();
    }
}
