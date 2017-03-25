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
        public static List<InstructionNode> SearchBackwardsForDataflowInstrcutions(Predicate<InstructionNode> predicate,
       InstructionNode startInstruction)
        {
            bool allPathsHaveAMatch;
            List<InstructionNode> foundBackInstructions = SafeSearchBackwardsForDataflowInstrcutions(predicate, startInstruction, out allPathsHaveAMatch);
            if (foundBackInstructions.Count == 0 || !allPathsHaveAMatch)
            {
                throw new Exception("Reached first instWrapper without correct one found");
            }
            return foundBackInstructions;
        }

        public static List<InstructionNode> SafeSearchBackwardsForDataflowInstrcutions(Predicate<InstructionNode> predicate,
        InstructionNode currentNode, out bool allPathsFoundAMatch, List<InstructionNode> visitedInstructions = null)
        {
            if (visitedInstructions == null)
            {
                visitedInstructions = new List<InstructionNode>();
            }
            while (true)
            {
                if (visitedInstructions.Contains(currentNode))
                {
                    allPathsFoundAMatch = false;
                    return new List<InstructionNode>();
                }
                else
                {
                    visitedInstructions.Add(currentNode);
                }
                allPathsFoundAMatch = true;
                var foundInstructions = new List<InstructionNode>();
                if (predicate.Invoke(currentNode))
                {
                    return new List<InstructionNode>() { currentNode };
                }
                if (currentNode.ProgramFlowBackRoutes.Count == 1)
                {
                    currentNode = currentNode.ProgramFlowBackRoutes[0];
                }
                else
                {
                    break;
                }
            }
            if (currentNode.ProgramFlowBackRoutes.Count == 0)
            {
                allPathsFoundAMatch = false;
                return new List<InstructionNode>();
            }
            List<InstructionNode> subBranchFoundNodes = new List<InstructionNode>();
            foreach (var backNode in currentNode.ProgramFlowBackRoutes)
            {
                bool pathContainsAMatch;
                IEnumerable<InstructionNode> branchindexes =
                    SafeSearchBackwardsForDataflowInstrcutions(predicate, backNode, out pathContainsAMatch, new List<InstructionNode>(visitedInstructions));
                subBranchFoundNodes.AddRange(branchindexes);
                if (!pathContainsAMatch)
                {
                    allPathsFoundAMatch = false;
                }
            }
            return subBranchFoundNodes;
        }
    }
}