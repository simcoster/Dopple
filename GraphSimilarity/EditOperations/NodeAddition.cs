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
        readonly List<GraphEdge> edgeAdditionsPending;

        public NodeAddition(List<InstructionWrapper> graph, InstructionWrapper node, List<GraphEdge> edgeAdditionsPending) : base(graph, node)
        {
            this.edgeAdditionsPending = edgeAdditionsPending;
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
            var addedEdges = new List<EdgeEditOperation>();
            List<GraphEdge> relatedEdgesToAdd = Node.BackDataFlowRelated.Select(x => new GraphEdge(x.Argument, Node))
                                                 .Concat(Node.ForwardDataFlowRelated.Select(x => new GraphEdge(x, Node)))
                                                 .ToList();
            IEnumerable<GraphEdge> triggeredEdgeAdds = edgeAdditionsPending.Where(x => x.DestinationNode == Node || x.SourceNode == Node);

            foreach (var edge in triggeredEdgeAdds.ToArray())
            {
                var edgeAddition = new EdgeAddition(graph, edge);
                edgeAddition.Edge = edge;
                edgeAdditionsPending.Remove(edge);
                relatedEdgesToAdd.Remove(edge);
                addedEdges.Add(edgeAddition);
            }

            foreach (var edgeLeftToBeAdded in relatedEdgesToAdd)
            {
                edgeAdditionsPending.Add(edgeLeftToBeAdded);
            }
            return addedEdges;
        }
    }
}
