using System;
using DoppleTry2.InstructionWrappers;
using GraphSimilarity.EditOperations;
using System.Collections.Generic;

namespace GraphSimilarity
{
    internal abstract class EdgeEditOperation : EditOperation
    {
        public EdgeEditOperation(List<InstructionWrapper> graph, GraphEdge edge) : base(graph)
        {
            this.Edge = edge;
        }

        public GraphEdge Edge { get; set; }
    }
}