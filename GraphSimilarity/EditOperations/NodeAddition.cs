using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionWrappers;

namespace GraphSimilarity.EditOperations
{
    internal class NodeAddition : NodeEditOperation
    {
        public NodeAddition(List<InstructionWrapper> graph) : base(graph)
        {
        }

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

        protected override List<EdgeEditOperation> GetEdgeOperations()
        {
            throw new NotImplementedException();
        }
    }
}
