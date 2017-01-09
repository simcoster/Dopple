using DoppleTry2;
using DoppleTry2.InstructionNodes;
using Northwoods.Go;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace DoppleGraph
{
    public class GoNodeWrapper
    {
        public GoNodeWrapper(GoTextNodeHoverable node, InstructionNode instructionWrapper)
        {
            Node = node;
            InstructionNode = instructionWrapper;
        }

        public GoTextNodeHoverable Node {  get; private set; }
        public InstructionNode InstructionNode { get; private set; }
        public int Index { get; set; }
        public float DisplayRow { get; set; }
        public int DisplayCol { get; set; }
        public List<GoNodeWrapper> LongestPath { get; set; } = new List<GoNodeWrapper>();
    }

 
}
