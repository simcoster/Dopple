using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;
using Dopple;

namespace GraphSimilarity.EditOperations
{
    class EdgeAddition : EdgeEditOperation
    {
        public EdgeAddition(List<InstructionNode> graph, GraphEdge edge) : base(graph, edge)
        {
        }

        public override int Cost
        {
            get
            {
                return 1;
            }
        }

        public override string Description
        {
            get
            {
                return "Added edge from " + Edge.SourceNode.InstructionIndex + " to " + Edge.DestinationNode.InstructionIndex; 
            }
        }

        public override string Name
        {
            get
            {
                return "Edge Addition";
            }
        }

        public override void Commit()
        {
            Edge.DestinationNode.DataFlowBackRelated.AddTwoWay(new IndexedArgument(Edge.Index, Edge.SourceNode,Edge.DestinationNode.DataFlowBackRelated));
        }
    }
}
