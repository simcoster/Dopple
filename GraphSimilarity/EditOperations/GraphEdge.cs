using DoppleTry2.InstructionWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarity.EditOperations
{
    public class GraphEdge
    {
        public InstructionWrapper SourceNode { get; private set; }
        public InstructionWrapper DestinationNode { get; private set; }

        public GraphEdge(InstructionWrapper sourceNode, InstructionWrapper destinationNode)
        {
            SourceNode = sourceNode;
            DestinationNode = destinationNode;
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
