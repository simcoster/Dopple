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

        public IEnumerable<Node> AddBackNodes(Node currentNode)
        {
            InstructionsWrappers[currentNode.InstructionWrapperIndex].WasTreated = true;
            if (!HasBackNodes)
            {
                currentNode.HasBackNodes = false;
                return new Node[0];
            }
            var indexes = GetBackRelatedIndices(currentNode.InstructionWrapperIndex, currentNode);
            foreach (var backRelatedIndex in indexes)
            {
                currentNode.BackNodes.Add(new Node(InstructionsWrappers[backRelatedIndex],backRelatedIndex));
            }
            return currentNode.BackNodes;
        }

        protected virtual bool HasBackNodes { get; } = true;

        protected abstract int[] GetBackRelatedIndices(int instructionIndex, Node currentNode);

        public abstract Code[] HandlesCodes { get; }

        protected int SearchBackwardsForInstrcution(Func<InstructionWrapper, bool> predicate, int startIndex)
        {
            return SearchBackwardsForInstrcutions(predicate,startIndex,1).First();
        }

        protected IEnumerable<int> SearchBackwardsForInstrcutions(Func<InstructionWrapper, bool> predicate, int startIndex, int howMany)
        {
            List<int> foundIndecies = new List<int>();

            int index = startIndex;
            for (int i = 0; i < howMany; i++)
            {
                int? foundIndex = SafeSearchBackwardsForInst(predicate, index);
                if (foundIndecies == null)
                {
                    throw new Exception("Searched instruction was not found");
                }
                index = foundIndex.Value;
                foundIndecies.Add(foundIndex.Value);
            }
            return foundIndecies;
        }


        protected int? SafeSearchBackwardsForInst(Func<InstructionWrapper, bool> predicate, int startItem)
        {
            for (var i = startItem - 1; i >= 0; i--)
            {
                if (predicate.Invoke(InstructionsWrappers[i]))
                {
                    return i;
                }
            }
            return null;
        }
    }
}
