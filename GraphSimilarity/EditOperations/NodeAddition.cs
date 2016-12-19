using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarity.EditOperations
{
    internal class NodeAddition : NodeEditOperation
    {
        public override int Cost
        {
            get
            {
                return 2;
            }
        }

        public override string Name
        {
            get
            {
                return "NodeAddition";
            }
        }
    }
}
