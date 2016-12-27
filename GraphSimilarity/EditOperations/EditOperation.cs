using System;
using DoppleTry2.InstructionNodes;
using GraphSimilarity.EditOperations;
using System.Collections.Generic;

namespace GraphSimilarity
{
    public abstract class EditOperation
    {
        public abstract string Name { get; }
        public abstract int Cost { get; }
        protected List<InstructionNode> graph;
        public abstract void Commit();
        public abstract string Description { get;}

        public EditOperation(List<InstructionNode> graph)
        {
            this.graph = graph;
        }
    }
}