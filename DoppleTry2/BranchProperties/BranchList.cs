using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.BranchPropertiesNS
{
    public class BranchList : List<BranchID>
    {
        public void AddDistinct(BranchID branch)
        {
            if (this.Contains(branch))
            {
                return;
            }
            else
            {
                Add(branch);
            }
        }

        public void AddRangeDistinct(IEnumerable<BranchID> branches)
        {
            foreach(var branch in branches)
            {
                this.AddDistinct(branch);
            }
        }
    }
}
