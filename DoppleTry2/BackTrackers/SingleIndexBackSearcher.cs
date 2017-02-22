using Dopple.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.BackTrackers
{
    public class SingleIndexBackSearcher
    {
        private List<InstructionNode> InstructionWrappers;
        public SingleIndexBackSearcher(List<InstructionNode> instructionsWrappers)
        {
            InstructionWrappers = instructionsWrappers;
        }

        public List<InstructionNode> SearchBackwardsForDataflowInstrcutions(Func<InstructionNode, bool> predicate,
       InstructionNode startInstruction)
        {
            List<InstructionNode> foundBackInstructions = SafeSearchBackwardsForDataflowInstrcutions(predicate, startInstruction);
            if (foundBackInstructions.Count == 0)
            {
                //TODO remove
                 //throw new Exception("Reached first instWrapper without correct one found");
            }
            return foundBackInstructions;
        }

        public List<InstructionNode> SafeSearchBackwardsForDataflowInstrcutions(Func<InstructionNode, bool> predicate,
           InstructionNode startInstruction)
        {
            return startInstruction.ProgramFlowBackRoutes.SelectMany(x => SafeSearchBackwardsForDataflowInstrcutions(InstructionWrappers, predicate, x, new List<InstructionNode>())).ToList();
        }

        public List<InstructionNode> SafeSearchBackwardsForDataflowInstrcutions(List<InstructionNode> instructionNodes, Func<InstructionNode, bool> predicate,
        InstructionNode startInstruction, List<InstructionNode> visitedInstructions)
        {
            if (visitedInstructions == null)
            {
                visitedInstructions = new List<InstructionNode>();
            }
            var foundInstructions = new List<InstructionNode>();
            int index = instructionNodes.IndexOf(startInstruction);
            if (index < 0)
            {
                throw new Exception("shouldn't get here");
            }

            var currInstruction = instructionNodes[index];
            if (visitedInstructions.Contains(currInstruction))
            {
                return new List<InstructionNode>();
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
                foreach (var instructionWrapper in currInstruction.ProgramFlowBackRoutes)
                {
                    IEnumerable<InstructionNode> branchindexes =
                        SafeSearchBackwardsForDataflowInstrcutions(instructionNodes, predicate, instructionWrapper, visitedInstructions);
                    foundInstructions.AddRange(branchindexes);
                }
            }
            return foundInstructions;
        }
    }
}
