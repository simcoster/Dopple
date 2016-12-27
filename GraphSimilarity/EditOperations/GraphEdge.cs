using DoppleTry2.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarity.EditOperations
{
    public class GraphEdge
    {
        public InstructionNode SourceNode { get; private set; }
        public InstructionNode DestinationNode { get; private set; }
        public int Index { get; private set; }

        public GraphEdge(InstructionNode sourceNode, InstructionNode destinationNode, int index)
        {
            SourceNode = sourceNode;
            DestinationNode = destinationNode;
            this.Index = index;
        }

        public override bool Equals(object obj)
        {
            if (obj is GraphEdge)
            {
                var otherEdge =(GraphEdge) obj;
                return SourceNode == otherEdge.SourceNode && DestinationNode == otherEdge.DestinationNode;

            }
            return base.Equals(obj);
        }
    }
}
