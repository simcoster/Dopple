using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionNodes;

namespace GraphSimilarity.EditOperations
{
    class EdgeSubstitution : EdgeEditOperation
    {
        GraphEdge replacementEdge;
        public EdgeSubstitution(List<InstructionNode> graph, GraphEdge edge, GraphEdge newEdge) : base(graph, edge)
        {
            this.replacementEdge = newEdge;
        }

        public override int Cost
        {
            get
            {
                return 0;
            }
        }

        public override string Description
        {
            get
            {
                return "replaced edge with source " + Edge.SourceNode.InstructionIndex + " and dest " + Edge.DestinationNode.InstructionIndex +
                        " with edge with source " + replacementEdge.SourceNode.InstructionIndex + " and dest " + replacementEdge.DestinationNode.InstructionIndex;
            }
        }

        public override string Name
        {
            get
            {
                return "Edge substitution";
            }
        }

        public override void Commit()
        {
            graph.First(x => x == Edge.DestinationNode).DataFlowBackRelated.AddTwoWay(Edge.SourceNode, Edge.Index);
        }
    }
}
