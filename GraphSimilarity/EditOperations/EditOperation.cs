using System;
using DoppleTry2.InstructionWrappers;

namespace GraphSimilarity
{
    internal abstract class EditOperation
    {
        public abstract string Name { get; }
        public abstract int Cost { get; }
    }

    internal abstract class NodeEditOperation : EditOperation
    {
        public InstructionWrapper InstructionWrapper { get; set; }
    }
}