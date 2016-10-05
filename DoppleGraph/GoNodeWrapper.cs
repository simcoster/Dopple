using DoppleTry2;
using Northwoods.Go;
using System.Collections.Generic;

namespace DoppleGraph
{
    public class GoNodeWrapper
    {
        public GoNodeWrapper(GoTextNodeHoverable node, InstructionWrapper instructionWrapper)
        {
            Node = node;
            InstructionWrapper = instructionWrapper;
        }

        public GoTextNodeHoverable Node {  get; private set; }
        public InstructionWrapper InstructionWrapper { get; private set; }
        public int Index { get; set; }
        public float DisplayRow { get; set; }
        public float DisplayCol { get; set; }
        public List<GoNodeWrapper> LongestPath { get; set; } = new List<GoNodeWrapper>();
    }

    public class GoTextNodeHoverable : GoTextNode
    {
        public override void OnLostSelection(GoSelection sel)
        {
            Form1.PaintNodeLinks(this);
            base.OnLostSelection(sel);
        }

        public override void OnGotSelection(GoSelection sel)
        {
            foreach (GoLink link in RightPort.Links)
            {
                link.PenWidth = 5;
                link.PenColor = System.Drawing.Color.Red;
            }
            foreach (GoLink link in LeftPort.Links)
            {
                link.PenWidth = 5;
                link.PenColor = System.Drawing.Color.Blue;
            }
            base.OnGotSelection(sel);
        }
    }
}
