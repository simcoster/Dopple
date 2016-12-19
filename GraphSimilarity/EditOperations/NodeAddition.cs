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
        public NodeAddition(List<InstructionWrapper> graph, Dictionary<InstructionWrapper,List<GraphEdge>> edgesToBeAdded) : base(graph)
        {
            EdgesToBeAdded = edgesToBeAdded;
        }

        readonly Dictionary<InstructionWrapper, List<GraphEdge>> EdgesToBeAdded;

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
            if (EdgesToBeAdded.ContainsKey(InstructionWrapper))
            {
                foreach(var edge in EdgesToBeAdded[InstructionWrapper])
                {
                    var edgeAddition = new EdgeAddition(graph,edge);
                    edgeAddition.Edge = edge;
                    addedEdges.Add();
                }
            }
        }
    }
}
