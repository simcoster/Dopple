using System;
using System.Linq;

namespace DoppleTry2
{
    public class InliningProperties
    {
        public bool Inlined { get; set; } = false;
        public int RecursionInstanceIndex = 0;
        public bool Recursive { get; set; } = false;
    }
}