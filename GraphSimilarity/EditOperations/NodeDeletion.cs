using DoppleTry2;
using DoppleTry2.InstructionWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarity.EditOperations
{
    internal class NodeDeletion : NodeEditOperation
    {
        public NodeDeletion(List<InstructionWrapper> graph, InstructionWrapper node) : base(graph, node)
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

        internal override List<InstructionWrapper> GetAddeddNodes()
        {
            return new List<InstructionWrapper>(){ };
        }

        internal override List<InstructionWrapper> GetDeletedNodes()
        {
            return new List<InstructionWrapper>() {Node };
        }

        internal override List<EdgeEditOperation> GetEdgeOperations()
        {
            var relatedEdgeOperations = new List<EdgeEditOperation>();
            InstructionWrapper nodeToRemove = Node;
            foreach (var backNode in nodeToRemove.BackDataFlowRelated.ToArray())
            {
                var tempEdgeDeletion = new EdgeDeletion(graph, new GraphEdge(backNode.Argument, nodeToRemove, backNode.ArgIndex));
                relatedEdgeOperations.Add(tempEdgeDeletion);
            }
            foreach (var forwardNode in nodeToRemove.ForwardDataFlowRelated.ToArray())
            {
                var tempEdgeDeletion = new EdgeDeletion(graph, new GraphEdge(forwardNode, nodeToRemove, forwardNode.BackDataFlowRelated.First(x => x.Argument == nodeToRemove).ArgIndex));
                relatedEdgeOperations.Add(tempEdgeDeletion);
            }
            return relatedEdgeOperations;
        }
    }
}
