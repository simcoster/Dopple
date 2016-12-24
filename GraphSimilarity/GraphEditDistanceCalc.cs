using DoppleTry2.InstructionWrappers;
using GraphSimilarity.EditOperations;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarity
{
    public static class GraphEditDistanceCalc
    {
        public const int NodeAdditionCost = 1;
        public const int NodeDeletionCost = 1;
        public const int NodeLabelSubstitutionCost = 1;

        public static EditPath GetEditDistance(List<InstructionWrapper> sourceGraph, List<InstructionWrapper> targetGraph)
        {
            http://www.springer.com/cda/content/document/cda_downloaddocument/9783319272511-c2.pdf?SGWID=0-0-45-1545097-p177820399
                 //step 1 = take all the first graph nodes
                 //step 2 = concider for each one, replacing with each one of the second graph
                 //step 3 = concider for each one, deleting
                 //continue until no more nodes left of the first graph

            var pathsToConcider = new SortedList<int, EditPath>();
            pathsToConcider.Add(0, new EditPath(sourceGraph, targetGraph));
            KeyValuePair<int, EditPath> cheapestPathValuePair = pathsToConcider.First();
            EditPath cheapestEditPath = cheapestPathValuePair.Value;
            while (true)
            {
                if (cheapestEditPath.SourceNodesLeftToResolve.Count != 0)
                {
                    List<CalculatedOperation> possibleOperations = GetPossibleSubsAndDelete(cheapestEditPath);
                    foreach (var possibleOperation in possibleOperations)
                    {
                        EditPath tempPathToConsider = cheapestEditPath.CloneWithEditOperation(possibleOperation);
                        pathsToConcider.Add(tempPathToConsider.CumelativeCostPlusHeuristic, tempPathToConsider);
                    }
                }
                else
                {
                    foreach (var nodeToAdd in cheapestEditPath.TargetNodesLeftToResolve)
                    {
                        CalculatedOperation nodeAddition = new NodeAddition(cheapestEditPath.Graph, nodeToAdd, cheapestEditPath.EdgeAdditionsPending).GetCalculated();
                        EditPath additionPath = cheapestEditPath.CloneWithEditOperation(nodeAddition);
                        pathsToConcider.Add(additionPath.CumelativeCostPlusHeuristic,additionPath);
                    }
                }
                pathsToConcider.Remove(cheapestPathValuePair.Key);
                cheapestPathValuePair = pathsToConcider.First();
                cheapestEditPath = cheapestPathValuePair.Value;
                if (cheapestEditPath.HeuristicCost == 0)
                {
                    return cheapestEditPath;
                }
            }
        }

        private static List<CalculatedOperation> GetPossibleSubsAndDelete(EditPath currentPath)
        {
            var calculatedOperations = new List<CalculatedOperation>();
            foreach (var node in currentPath.SourceNodesLeftToResolve)
            {
                foreach (var secondGraphNode in currentPath.TargetNodesLeftToResolve)
                {
                    var nodeSubstitution = new NodeSubstitution(currentPath.Graph, currentPath.EdgeAdditionsPending, node, secondGraphNode);
                    calculatedOperations.Add(nodeSubstitution.GetCalculated());
                }
                var nodeDeletion = new NodeDeletion(currentPath.Graph, node);
                calculatedOperations.Add(nodeDeletion.GetCalculated());
            }
            return calculatedOperations;
        }
    }
}
