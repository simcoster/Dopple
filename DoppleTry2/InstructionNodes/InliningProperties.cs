using System;
using System.Linq;

namespace DoppleTry2.InstructionNodes
{
    public class InliningProperties
    {
        public bool Inlined { get; set; } = false;
        public int RecursionSameLevelIndex = 0;
        public int RecursionLevel { get; set; } = 0;
        public bool Recursive { get; set; } = false;
    }
}