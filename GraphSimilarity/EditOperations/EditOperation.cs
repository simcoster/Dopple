using System;
using DoppleTry2.InstructionWrappers;
using GraphSimilarity.EditOperations;
using System.Collections.Generic;

namespace GraphSimilarity
{
    public abstract class EditOperation
    {
        public abstract string Name { get; }
        public abstract int Cost { get; }
        protected List<InstructionWrapper> graph;
        public abstract void Commit();
        public abstract string Description { get;}

        public EditOperation(List<InstructionWrapper> graph)
        {
            this.graph = graph;
        }
    }
}