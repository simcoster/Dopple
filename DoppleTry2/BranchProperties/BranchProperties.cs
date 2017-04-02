using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.BranchPropertiesNS
{
    public class BranchProperties
    {
        public List<BranchID> Branches { get; private set; } = new List<BranchID>();
        public MergingNodeProperties MergingNodeProperties { get; private set; } = new MergingNodeProperties();
    }
}
