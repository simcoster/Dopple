using System;
using Dopple.InstructionNodes;
using GraphSimilarity.EditOperations;
using System.Collections.Generic;

namespace GraphSimilarity
{
    public abstract class EdgeEditOperation : EditOperation
    {
        public EdgeEditOperation(List<InstructionNode> graph, GraphEdge edge) : base(graph)
        {
            this.Edge = edge;
        }

        public GraphEdge Edge { get; set; }
    }
}