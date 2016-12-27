using DoppleTry2;
using DoppleTry2.InstructionNodes;

namespace ForceBasedDiagram
{
    internal class GoNodeWrapper
    {
        public SpotNode SpotNode;
        public InstructionNode InstWrapper;

        public int Index { get; internal set; }

        public GoNodeWrapper(SpotNode spotNode, InstructionNode instWrapper)
        {
            SpotNode = spotNode;
            InstWrapper = instWrapper;
        }
    }
}