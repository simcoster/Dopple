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
            bool allPathsHaveAMatch;
            List<InstructionNode> foundBackInstructions = SafeSearchBackwardsForDataflowInstrcutions(predicate, startInstruction,out allPathsHaveAMatch);
            if (foundBackInstructions.Count == 0 || !allPathsHaveAMatch)
            {
                throw new Exception("Reached first instWrapper without correct one found");
            }
            return foundBackInstructions;
        }

        public static List<InstructionNode> SafeSearchBackwardsForDataflowInstrcutions(Func<InstructionNode, bool> predicate,
        InstructionNode startInstruction , out bool allPathsFoundAMatch,  List<InstructionNode> visitedInstructions = null)
        {
            if (visitedInstructions == null)
            {
                visitedInstructions = new List<InstructionNode>();
            }
            var foundInstructions = new List<InstructionNode>();
            if (visitedInstructions.Contains(startInstruction))
            {
                allPathsFoundAMatch = false;
                return new List<InstructionNode>();
            }
            else
            {
                visitedInstructions.Add(startInstruction);
            }
            allPathsFoundAMatch = true;
            if (predicate.Invoke(startInstruction))
            {
                foundInstructions.Add(startInstruction);
            }
            else
            {
                if (startInstruction.ProgramFlowForwardRoutes.Count == 0)
                {
                    allPathsFoundAMatch = false;
                }
                foreach (var instructionWrapper in startInstruction.ProgramFlowBackRoutes)
                {
                    bool pathContainsAMatch;
                    IEnumerable<InstructionNode> branchindexes =
                        SafeSearchBackwardsForDataflowInstrcutions(predicate, instructionWrapper, out pathContainsAMatch, visitedInstructions);
                    foundInstructions.AddRange(branchindexes);
                    if (!pathContainsAMatch)
                    {
                        allPathsFoundAMatch = false;
                    }
                }
            }
            return foundInstructions;
        }
    }
}
