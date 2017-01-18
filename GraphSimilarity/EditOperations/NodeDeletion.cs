using DoppleTry2;
using DoppleTry2.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarity.EditOperations
{
    internal class NodeDeletion : NodeEditOperation
    {
        public NodeDeletion(List<InstructionNode> graph, InstructionNode node) : base(graph, node)
        {
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
                return "Delete node with index " + Node.InstructionIndex;
            }
        }

        public override string Name
        {
            get
            {
                return "NodeDeletion";
            }
        }

        public override void Commit()
        {
            graph.Remove(Node);
        }

        internal override List<InstructionNode> GetAddeddNodes()
        {
            return new List<InstructionNode>(){ };
        }

        internal override List<InstructionNode> GetDeletedNodes()
        {
            return new List<InstructionNode>() {Node };
        }

        internal override List<EdgeEditOperation> GetEdgeOperations()
        {
            var relatedEdgeOperations = new List<EdgeEditOperation>();
            InstructionNode nodeToRemove = Node;
            foreach (var backNode in nodeToRemove.DataFlowBackRelated.ToArray())
            {
                var tempEdgeDeletion = new EdgeDeletion(graph, new GraphEdge(backNode.Argument, nodeToRemove, backNode.ArgIndex));
                relatedEdgeOperations.Add(tempEdgeDeletion);
            }
            foreach (var forwardNode in nodeToRemove.DataFlowForwardRelated.ToArray())
            {
                var tempEdgeDeletion = new EdgeDeletion(graph, new GraphEdge(forwardNode.Argument, nodeToRemove, forwardNode.ArgIndex));
                relatedEdgeOperations.Add(tempEdgeDeletion);
            }
            return relatedEdgeOperations;
        }
    }
}
