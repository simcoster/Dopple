using DoppleTry2.InstructionWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.BackTrackers
{
    public class SingleIndexBackSearcher
    {
        private List<InstructionWrapper> InstructionWrappers;
        public SingleIndexBackSearcher(List<InstructionWrapper> instructionsWrappers)
        {
            InstructionWrappers = instructionsWrappers;
        }

        public List<InstructionWrapper> SearchBackwardsForDataflowInstrcutions(Func<InstructionWrapper, bool> predicate,
       InstructionWrapper startInstruction)
        {
            List<InstructionWrapper> foundBackInstructions = SafeSearchBackwardsForDataflowInstrcutions(predicate, startInstruction);
            if (foundBackInstructions.Count == 0)
            {
                throw new Exception("Reached first instWrapper without correct one found");
            }
            return foundBackInstructions;
        }

        public List<InstructionWrapper> SafeSearchBackwardsForDataflowInstrcutions(Func<InstructionWrapper, bool> predicate,
           InstructionWrapper startInstruction)
        {
            return startInstruction.BackProgramFlow.SelectMany(x => SafeSearchBackwardsForDataflowInstrcutions(InstructionWrappers, predicate, x, new List<InstructionWrapper>())).ToList();
        }

        public List<InstructionWrapper> SafeSearchBackwardsForDataflowInstrcutions(List<InstructionWrapper> InstructionWrappers, Func<InstructionWrapper, bool> predicate,
        InstructionWrapper startInstruction, List<InstructionWrapper> visitedInstructions)
        {
            if (visitedInstructions == null)
            {
                visitedInstructions = new List<InstructionWrapper>();
            }
            var foundInstructions = new List<InstructionWrapper>();
            int index = InstructionWrappers.IndexOf(startInstruction);
            if (index < 0)
                throw new Exception("shouldn't get here");

            var currInstruction = InstructionWrappers[index];
            if (visitedInstructions.Contains(currInstruction))
            {
                return new List<InstructionWrapper>();
            }
            else
            {
                visitedInstructions.Add(currInstruction);
            }

            if (predicate.Invoke(currInstruction))
            {
                foundInstructions.Add(currInstruction);
            }

            else
            {
                foreach (var instructionWrapper in currInstruction.BackProgramFlow)
                {
                    IEnumerable<InstructionWrapper> branchindexes =
                        SafeSearchBackwardsForDataflowInstrcutions(InstructionWrappers, predicate, instructionWrapper, visitedInstructions);
                    foundInstructions.AddRange(branchindexes);
                }
            }
            return foundInstructions;
        }
    }
}
