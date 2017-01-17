using DoppleTry2.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarity.EditOperations
{
    internal class NodeSubstitution : NodeEditOperation
    {
        private readonly List<GraphEdge> edgeAdditionsPending;
        private readonly InstructionNode NodeToReplaceWith;

        public NodeSubstitution(List<InstructionNode> graph, List<GraphEdge> edgeAdditionsPending, InstructionNode node, InstructionNode replacedWith) : base(graph, node)
        {
            this.NodeToReplaceWith = replacedWith;
            this.edgeAdditionsPending = edgeAdditionsPending;
        }

        public override int Cost
        {
            get
            {
                if (Node.Instruction.OpCode.Code == NodeToReplaceWith.Instruction.OpCode.Code)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }

            }
        }

        public override string Description
        {
            get
            {
                return "Replaced node " + Node.InstructionIndex + " with node " + NodeToReplaceWith.InstructionIndex;
            }
        }

        public override string Name
        {
            get
            {
                return "NodeSubstitution";
            }
        }

        public override void Commit()
        {
            graph.Remove(Node);
            graph.Add(NodeToReplaceWith);
        }

        internal override List<InstructionNode> GetAddeddNodes()
        {
            return new List<InstructionNode>() { NodeToReplaceWith };
        }

        internal override List<InstructionNode> GetDeletedNodes()
        {
            return new List<InstructionNode>() { Node };
        }

        internal override List<EdgeEditOperation> GetEdgeOperations()
        {
            var edgeOperations = new List<EdgeEditOperation>();
            var originalNodeEdges = GetAllEdges(Node);
            var replacementNodeEdges = GetAllEdges(Node);
            foreach (var originalNodeIsDestEdge in originalNodeEdges.NodeIsDestinationEdges)
            {
                var destNodeEdge = replacementNodeEdges.NodeIsDestinationEdges.FirstOrDefault(x => x.SourceNode.Instruction.OpCode.Code == originalNodeIsDestEdge.SourceNode.Instruction.OpCode.Code);
                if (destNodeEdge != null)
                {
                }
            }
            var nodeDeletion = new NodeDeletion(graph, Node);
            var nodeAddition = new NodeAddition(graph, NodeToReplaceWith, edgeAdditionsPending);
            return nodeDeletion.GetEdgeOperations().Concat(nodeAddition.GetEdgeOperations()).ToList();
        }

        private Edges GetAllEdges(InstructionNode node)
        {
            Edges edges = new Edges();
            edges.NodeIsDestinationEdges =  node.DataFlowBackRelated.Select(x => new GraphEdge(x.Argument, node, x.ArgIndex)).ToList();
            edges.NodeIsSourceEdges = node.DataFlowForwardRelated.Select(x => new GraphEdge(node, x, x.DataFlowBackRelated.First(y => y.Argument == node).ArgIndex)).ToList();
            return edges;
        }

        internal class Edges
        {
            public List<GraphEdge> NodeIsSourceEdges { get; set; }
            public List<GraphEdge> NodeIsDestinationEdges { get; set; }
        }
    }
}
