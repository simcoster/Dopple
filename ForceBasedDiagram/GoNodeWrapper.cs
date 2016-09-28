using DoppleTry2;

namespace ForceBasedDiagram
{
    internal class GoNodeWrapper
    {
        public SpotNode SpotNode;
        public InstructionWrapper InstWrapper;

        public int Index { get; internal set; }

        public GoNodeWrapper(SpotNode spotNode, InstructionWrapper instWrapper)
        {
            SpotNode = spotNode;
            InstWrapper = instWrapper;
        }
    }
}