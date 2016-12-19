using System;
using DoppleTry2.InstructionWrappers;
using GraphSimilarity.EditOperations;
using System.Collections.Generic;

namespace GraphSimilarity
{
    internal abstract class EditOperation
    {
        public abstract string Name { get; }
        public abstract int Cost { get; }
        protected List<InstructionWrapper> graph;

        public EditOperation(List<InstructionWrapper> graph)
        {
            this.graph = graph;
        }
    }
}