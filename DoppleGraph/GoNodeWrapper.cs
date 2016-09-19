using DoppleTry2;
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
    }
}
