using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace DoppleTry2
{
    public class FunctionGraph
    {
        List<Node> EndNodes { get; set; }
    }

    public class Node
    {
        public Node(InstructionWrapper instruction, int instructionWrapperIndex)
        {
            InstructionWrapper = instruction;
            InstructionWrapperIndex = instructionWrapperIndex;
        }
        public InstructionWrapper InstructionWrapper { get; private set; }
        public int InstructionWrapperIndex { get; set; }
        internal virtual List<Node> ForwardNodes { get; set; }
        internal virtual List<Node> BackNodes { get; set; } = new List<Node>();
        public bool HasBackNodes { get; set; } = false;
    }
}