using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dopple.InstructionNodes
{
    public class InliningProperties
    {
        public bool Inlined { get; set; } = false;
        public int RecursionInstanceIndex = 0;
        public List<MethodDefinition> CallSequence { get; set; }
        public bool Recursive { get; set; } = false;
        public InlineableCallNode CallNode { get; set; }
    }
}