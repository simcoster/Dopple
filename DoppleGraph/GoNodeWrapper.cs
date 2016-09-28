using DoppleTry2;
using Northwoods.Go;
using System.Collections.Generic;

namespace DoppleGraph
{
    public class GoNodeWrapper
    {
        public GoNodeWrapper(GoTextNode node, InstructionWrapper instructionWrapper)
        {
            Node = node;
            InstructionWrapper = instructionWrapper;
        }

        public GoTextNode Node {  get; private set; }
        public InstructionWrapper InstructionWrapper { get; private set; }
        public int Index { get; set; }
        public float DisplayRow { get; set; }
        public float DisplayCol { get; set; }
        public List<GoNodeWrapper> LongestPath { get; set; } = new List<GoNodeWrapper>();
    }
}
