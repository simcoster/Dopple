using System;
using System.Linq;

namespace Dopple.InstructionNodes
{
    public class InliningProperties
    {
        public bool Inlined { get; set; } = false;
        public int SameMethodCallIndex = 0;
        public int RecursionLevel { get; set; } = 0;
        public bool Recursive { get; set; } = false;
        public InlineableCallNode CallNode { get; set; }
    }
}