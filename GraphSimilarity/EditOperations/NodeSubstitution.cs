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
        private readonly InstructionWrapper replacedWith;

        public NodeSubstitution(List<InstructionWrapper> graph, List<GraphEdge> edgeAdditionsPending, InstructionWrapper node, InstructionWrapper replacedWith) : base(graph, node)
        {
            this.replacedWith = replacedWith;
            this.edgeAdditionsPending = edgeAdditionsPending;
        }

        public override int Cost
        {
            get
            {
                if (Node.Instruction.OpCode.Code == replacedWith.Instruction.OpCode.Code)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }

            }
        }

        public override string Name
        {
            get
            {
                return "NodeSubstitution";
            }
        }

        protected override List<EdgeEditOperation> GetEdgeOperations()
        {
            var nodeDeletion = new NodeDeletion(graph, Node);
            var nodeAddition = new NodeAddition(graph, replacedWith, edgeAdditionsPending);
            return nodeDeletion.G
        }
    }
}
