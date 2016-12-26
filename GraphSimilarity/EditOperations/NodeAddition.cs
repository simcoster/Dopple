using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionWrappers;
using System.Diagnostics;

namespace GraphSimilarity.EditOperations
{
    internal class NodeAddition : NodeEditOperation
    {
        readonly List<GraphEdge> edgeAdditionsPending;
        private List<GraphEdge> edgeAddsToBeTriggered;
        private IEnumerable<GraphEdge> triggeredEdgeAdds;

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

        public override string Description
        {
            get
            {
                return "Added node with index " + Node.InstructionIndex;
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
            foreach (var triggeredEdgeToAdd in triggeredEdgeAdds)
            {
                edgeAdditionsPending.Remove(triggeredEdgeToAdd);
            }
            foreach (var edgeLeftToBeAdded in edgeAddsToBeTriggered)
            {
                edgeAdditionsPending.Add(edgeLeftToBeAdded);
            }

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
            var problematics = Node.ForwardDataFlowRelated.Where(x => !x.BackDataFlowRelated.Any(y => y.Argument == Node)).ToList();
            if (problematics.Count > 0)
            {
                Debugger.Break();
            }
            List<GraphEdge> relatedEdgesToAdd = Node.BackDataFlowRelated.Select(x => new GraphEdge(x.Argument, Node, x.ArgIndex))
                                                 .Concat(Node.ForwardDataFlowRelated.Select(x => new GraphEdge(x, Node, x.BackDataFlowRelated.First(y => y.Argument ==Node).ArgIndex)))
                                                 .ToList();
            triggeredEdgeAdds = edgeAdditionsPending.Where(x => x.DestinationNode == Node || x.SourceNode == Node);
            edgeAddsToBeTriggered = relatedEdgesToAdd.Except(triggeredEdgeAdds).ToList();
            foreach (var triggeredEdgeToAdd in triggeredEdgeAdds.ToArray())
            {
                var edgeAddition = new EdgeAddition(graph, triggeredEdgeToAdd);
                edgeAddition.Edge = triggeredEdgeToAdd;
                edgeAdditionsPending.Remove(triggeredEdgeToAdd);
                addedEdges.Add(edgeAddition);
            }
            return addedEdges;
        }
    }
}
