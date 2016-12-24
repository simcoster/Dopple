using DoppleTry2.InstructionWrappers;
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
        private readonly InstructionWrapper NodeToReplaceWith;

        public NodeSubstitution(List<InstructionWrapper> graph, List<GraphEdge> edgeAdditionsPending, InstructionWrapper node, InstructionWrapper replacedWith) : base(graph, node)
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

        internal override List<InstructionWrapper> GetAddeddNodes()
        {
            return new List<InstructionWrapper>() { NodeToReplaceWith };
        }

        internal override List<InstructionWrapper> GetDeletedNodes()
        {
            return new List<InstructionWrapper>() { Node };
        }

        internal override List<EdgeEditOperation> GetEdgeOperations()
        {
            var nodeDeletion = new NodeDeletion(graph, Node);
            var nodeAddition = new NodeAddition(graph, NodeToReplaceWith, edgeAdditionsPending);
            return nodeDeletion.GetEdgeOperations().Concat(nodeAddition.GetEdgeOperations()).ToList();
        }
    }
}
