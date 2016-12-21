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

        public override void Commit()
        {
            graph.Add(Node);
        }

        internal override List<InstructionWrapper> GetAddeddNodes()
        {
            return new List<InstructionWrapper>() { Node };
        }

        internal override List<InstructionWrapper> GetDeletedNodes()
        {
            return new List<InstructionWrapper>();
        }

        internal override List<EdgeEditOperation> GetEdgeOperations()
        {
            var addedEdges = new List<EdgeEditOperation>();
            List<GraphEdge> relatedEdgesToAdd = Node.BackDataFlowRelated.Select(x => new GraphEdge(x.Argument, Node, x.ArgIndex))
                                                 .Concat(Node.ForwardDataFlowRelated.Select(x => new GraphEdge(x, Node, x.BackDataFlowRelated.First(y => y.Argument ==x).ArgIndex)))
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
