using Dopple.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.BackTracers
{
    public static class SingleIndexBackSearcher
    {
        public static List<InstructionNode> SearchBackwardsForDataflowInstrcutions(Func<InstructionNode, bool> predicate,
       InstructionNode startInstruction)
        {
            List<InstructionNode> foundBackInstructions = SafeSearchBackwardsForDataflowInstrcutions(predicate, startInstruction);
            if (foundBackInstructions.Count == 0)
            {
                throw new Exception("Reached first instWrapper without correct one found");
            }
            return foundBackInstructions;
        }

        public static List<InstructionNode> SafeSearchBackwardsForDataflowInstrcutions(Func<InstructionNode, bool> predicate,
           InstructionNode startInstruction)
        {
            return startInstruction.ProgramFlowBackRoutes.SelectMany(x => SafeSearchBackwardsForDataflowInstrcutions(predicate, x, new List<InstructionNode>())).ToList();
        }

        public static List<InstructionNode> SafeSearchBackwardsForDataflowInstrcutions(Func<InstructionNode, bool> predicate,
        InstructionNode startInstruction, List<InstructionNode> visitedInstructions)
        {
            if (visitedInstructions == null)
            {
                visitedInstructions = new List<InstructionNode>();
            }
            var foundInstructions = new List<InstructionNode>();

            if (visitedInstructions.Contains((InstructionNode) startInstruction))
            {
                return new List<InstructionNode>();
            }
            else
            {
                visitedInstructions.Add((InstructionNode) startInstruction);
            }

            if (predicate.Invoke((InstructionNode) startInstruction))
            {
                foundInstructions.Add((InstructionNode) startInstruction);
            }

            else
            {
                foreach (var instructionWrapper in startInstruction.ProgramFlowBackRoutes)
                {
                    IEnumerable<InstructionNode> branchindexes =
                        SafeSearchBackwardsForDataflowInstrcutions(predicate, instructionWrapper, visitedInstructions);
                    foundInstructions.AddRange(branchindexes);
                }
            }
            return foundInstructions;
        }
    }
}
