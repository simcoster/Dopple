﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;
using System.Diagnostics;

namespace GraphSimilarity.EditOperations
{
    internal class NodeAddition : NodeEditOperation
    {
        readonly List<GraphEdge> edgeAdditionsPending;
        private List<GraphEdge> edgeAddsToBeTriggered;
        private IEnumerable<GraphEdge> triggeredEdgeAdds;

        public NodeAddition(List<InstructionNode> graph, InstructionNode node, List<GraphEdge> edgeAdditionsPending) : base(graph, node)
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

        internal override List<InstructionNode> GetAddeddNodes()
        {
            return new List<InstructionNode>() { Node };
        }

        internal override List<InstructionNode> GetDeletedNodes()
        {
            return new List<InstructionNode>();
        }

        internal override List<EdgeEditOperation> GetEdgeOperations()
        {
            var addedEdges = new List<EdgeEditOperation>();
            var problematics = Node.DataFlowForwardRelated.Where(x => !x.Argument.DataFlowBackRelated.Any(y => y.Argument == Node)).ToList();
            if (problematics.Count > 0)
            {
                Debugger.Break();
            }
            List<GraphEdge> relatedEdgesToAdd = Node.DataFlowBackRelated.Select(x => new GraphEdge(x.Argument, Node, x.ArgIndex))
                                                 .Concat(Node.DataFlowForwardRelated.Select(x => new GraphEdge(x.Argument, Node, x.ArgIndex)))
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
