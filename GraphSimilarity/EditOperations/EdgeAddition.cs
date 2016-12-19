using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionWrappers;

namespace GraphSimilarity.EditOperations
{
    class EdgeAddition : EdgeEditOperation
    {
        public EdgeAddition(List<InstructionWrapper> graph, GraphEdge edge) : base(graph, edge)
        {
        }

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
                return "Edge Addition";
            }
        }
    }
}
