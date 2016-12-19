using System;
using DoppleTry2.InstructionWrappers;
using GraphSimilarity.EditOperations;
using System.Collections.Generic;

namespace GraphSimilarity
{
    internal abstract class EdgeEditOperation : EditOperation
    {
        public EdgeEditOperation(List<InstructionWrapper> graph) : base(graph)
        {
        }

        public GraphEdge Edge { get; set; }
    }
}