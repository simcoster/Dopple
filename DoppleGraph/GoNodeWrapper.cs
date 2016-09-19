using DoppleTry2;
using Northwoods.Go;

namespace DoppleGraph
{
    public class GoNodeWrapper
    {
        public GoNodeWrapper(GoNode node, InstructionWrapper instructionWrapper)
        {
            Node = node;
            InstructionWrapper = instructionWrapper;
        }

        public GoNode Node {  get; private set; }
        public InstructionWrapper InstructionWrapper { get; private set; }
    }
}
