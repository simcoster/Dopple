using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;
using Dopple;

namespace GraphSimilarity.EditOperations
{
    class EdgeDeletion : EdgeEditOperation
    {
        public EdgeDeletion(List<InstructionNode> graph, GraphEdge edge) : base(graph, edge)
        {
        }

        public override int Cost
        {
            get
            {
                return 1;
            }
        }

        public override string Description
        {
            get
            {
                return "Deleted edge from " + Edge.SourceNode.InstructionIndex + " to " + Edge.DestinationNode.InstructionIndex;
            }
        }

        public override string Name
        {
            get
            {
                return "Edge Deletion";
            }
        }

        public override void Commit()
        {
            CoupledIndexedArgList backNodes = Edge.DestinationNode.DataFlowBackRelated;
            backNodes.RemoveTwoWay(backNodes.First(x => x.Argument == Edge.SourceNode));
        }
    }
}
