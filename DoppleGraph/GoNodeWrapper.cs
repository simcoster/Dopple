using DoppleTry2;
using Northwoods.Go;
using System;
using System.Collections.Generic;
using System.Drawing;

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
        public int DisplayCol { get; set; }
        public List<GoNodeWrapper> LongestPath { get; set; } = new List<GoNodeWrapper>();
    }

    public class GoTextNodeHoverable : GoTextNode
    {
        public event EventHandler Selected;
        public event EventHandler UnSelected;

        public override void OnLostSelection(GoSelection sel)
        {
            foreach(GoLink link in Links)
            {
                link.PenWidth = 3;
                link.HighlightPenWidth = 0;
            }
            UnSelected(this, null);
            base.OnLostSelection(sel);
        }
        
        public override bool OnEnterLeave(GoObject from, GoObject to, GoView view)
        {

            return base.OnEnterLeave(from, to, view);
        }

        public override void OnGotSelection(GoSelection sel)
        {
            foreach (GoLink link in RightPort.Links)
            {
                link.PenWidth = 5;
                link.HighlightPenWidth = 8;
                link.HighlightPenColor = System.Drawing.Color.Red;
            }
            foreach (GoLink link in LeftPort.Links)
            {
                link.HighlightPenWidth = 8;
                link.PenWidth = 5;
                link.HighlightPenColor = System.Drawing.Color.Blue;
            }
            Selected(this, null);
            base.OnGotSelection(sel);
        }
    }
}
