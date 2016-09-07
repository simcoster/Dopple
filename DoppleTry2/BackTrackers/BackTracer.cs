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

        public void AddBackDataflowConnections(InstructionWrapper currentInst)
        {
            currentInst.WasTreated = true;
            if (!HasBackDataflowNodes)
            {
                currentInst.HasBackRelated = false;
                return;
            }
            var indexes = GetDataflowBackRelatedIndices(InstructionsWrappers.IndexOf(currentInst));
            foreach (var backRelatedIndex in indexes)
            {
                currentInst.BackDataFlowRelated.Add(InstructionsWrappers[backRelatedIndex]);
            }
        }

        protected virtual bool HasBackDataflowNodes { get; } = true;

        protected abstract IEnumerable<int> GetDataflowBackRelatedIndices(int instructionIndex);

        public abstract Code[] HandlesCodes { get; }

        protected IEnumerable<int> SearchBackwardsForDataflowInstrcutions(Func<InstructionWrapper, bool> predicate,
            int startIndex)
        {
            IEnumerable<int> indexes;
            if (TrySearchBackwardsForDataflowInstrcutions(predicate, startIndex, out indexes) == false)
            {
                throw new Exception("Reached first instruction without correct one found");
            }
            else
            {
                return indexes;
            }
        }

        protected bool TrySearchBackwardsForDataflowInstrcutions(Func<InstructionWrapper, bool> predicate,
           int startIndex, out IEnumerable<int> indexes)
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
                else if (currInstruction.BackProgramFlow.Count == 1)
                {
                    if (InstructionsWrappers.IndexOf(currInstruction.BackProgramFlow[0]) == 1)
                    {
                        indexes = null;
                        return false;
                    }
                    index--;
                }
                else
                {
                    foreach (var instructionWrapper in currInstruction.BackProgramFlow)
                    {
                        IEnumerable<int> branchindexes;
                        TrySearchBackwardsForDataflowInstrcutions(predicate,
                            InstructionsWrappers.IndexOf(instructionWrapper), out branchindexes);
                        foundIndexes.AddRange(branchindexes);
                    }
                }
            }
            indexes = foundIndexes;
            return true;
        }
    }
}
