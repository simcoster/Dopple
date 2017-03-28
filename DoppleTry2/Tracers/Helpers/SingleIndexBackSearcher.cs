using Dopple.InstructionNodes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.BackTracers
{
    public static class SingleIndexBackSearcher
    {
        public static List<InstructionNode> SafeSearchBackwardsForDataflowInstrcutions(Predicate<InstructionNode> predicate,
        InstructionNode currentNode, out bool allPathsFoundAMatch)
        {
            ConcurrentBag<InstructionNode> foundNodes = new ConcurrentBag<InstructionNode>();
            allPathsFoundAMatch = SearchBackwardsForDataflowInstrcutionsRec(predicate, currentNode, foundNodes);
            return foundNodes.Distinct().ToList();
        }

        private static bool SearchBackwardsForDataflowInstrcutionsRec(Predicate<InstructionNode> predicate,
        InstructionNode currentNode, ConcurrentBag<InstructionNode> foundNodes, List<InstructionNode> visitedInstructions = null)
        {
            if (visitedInstructions == null)
            {
                visitedInstructions = new List<InstructionNode>();
            }
            if (foundNodes == null)
            {
                throw new Exception("Must supply an initlized collection");
            }
            while (true)
            {
                lock(visitedInstructions)
                {
                    if (visitedInstructions.Contains(currentNode))
                    {
                        return false;
                    }
                    else
                    {
                        visitedInstructions.Add(currentNode);
                    }
                }
                if (predicate.Invoke(currentNode))
                {
                    foundNodes.Add(currentNode);
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
                return false;
            }
            return currentNode.ProgramFlowBackRoutes.AsParallel().Any(x => SearchBackwardsForDataflowInstrcutionsRec(predicate, x, foundNodes, visitedInstructions));
        }
    }
}