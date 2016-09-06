using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    public abstract class BackTracer
    {
        protected InstructionWrapper Instruction;
        protected readonly List<InstructionWrapper> InstructionsWrappers;

        protected readonly IEnumerable<OpCode> AllOpCodes =
            typeof(OpCodes).GetFields().Select(x => x.GetValue(null)).Cast<OpCode>();

        protected BackTracer(List<InstructionWrapper> instructionsWrappers)
        {
            InstructionsWrappers = instructionsWrappers;
        }

        public IEnumerable<Node> AddBackDataflowNodes(Node currentNode)
        {
            InstructionsWrappers[currentNode.InstructionWrapperIndex].WasTreated = true;
            if (!HasBackDataflowNodes)
            {
                currentNode.HasBackNodes = false;
                return new Node[0];
            }
            var indexes = GetDataflowBackRelatedIndices(currentNode.InstructionWrapperIndex, currentNode);
            foreach (var backRelatedIndex in indexes)
            {
                currentNode.BackNodes.Add(new Node(InstructionsWrappers[backRelatedIndex], backRelatedIndex));
            }
            return currentNode.BackNodes;
        }

        protected virtual bool HasBackDataflowNodes { get; } = true;

        protected abstract IEnumerable<int> GetDataflowBackRelatedIndices(int instructionIndex, Node currentNode);

        public abstract Code[] HandlesCodes { get; }

        protected IEnumerable<int> SearchBackwardsForDataflowInstrcutions(Func<InstructionWrapper, bool> predicate,
            int startIndex)
        {
            List<int> foundIndexes = new List<int>();
            int index = startIndex;
            bool found = false;
            while (found == false)
            {
                var currInstruction = InstructionsWrappers[index];
                if (predicate.Invoke(currInstruction))
                {
                    foundIndexes.Add(index);
                    found = true;
                }
                else if (currInstruction.Back.Count == 1)
                {
                    if (InstructionsWrappers.IndexOf(currInstruction.Back[0]) == 1)
                    {
                        throw new Exception("Reached first instruction without correct one found");
                    }
                    index--;
                }
                else
                {
                    foreach (var instructionWrapper in currInstruction.Back)
                    {
                        var branchindexes = SearchBackwardsForDataflowInstrcutions(predicate,
                            InstructionsWrappers.IndexOf(instructionWrapper));
                        foundIndexes.AddRange(branchindexes);
                    }
                }
            }
            return foundIndexes;
        }
    }
}
