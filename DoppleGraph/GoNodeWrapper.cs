﻿using DoppleTry2;
using Northwoods.Go;

namespace DoppleGraph
{
    public class GoNodeWrapper
    {
        public GoNodeWrapper(GoBasicNode node, InstructionWrapper instructionWrapper)
        {
            Node = node;
            InstructionWrapper = instructionWrapper;
        }

        public GoBasicNode Node {  get; private set; }
        public InstructionWrapper InstructionWrapper { get; private set; }
        public int Index { get; set; }
        public float LineNum { get; set; } = 0;
        public float ColNum { get; set; } = 0;

    }
}
