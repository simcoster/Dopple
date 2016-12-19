using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarity.EditOperations
{
    class EdgeDeletion : EditOperation
    {
        public override int Cost
        {
            get
            {
                return 1;
            }
        }

        public override string Name
        {
            get
            {
                return "Edge Deletion";
            }
        }
    }
}
