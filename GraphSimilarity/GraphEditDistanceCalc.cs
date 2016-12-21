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
    class GraphEditDistanceCalc
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

            var pathsToConcider = new List<EditPath>();
            var currEditPath = new EditPath(sourceGraph, targetGraph);
            while (true)
            {
                if (currEditPath.SourceNodesLeftToResolve.Count !=0)
                {
                    foreach (var node in currEditPath.Graph)
                    {
                        List<CalculatedOperation> possibleOperations = GetPossibleSubsAndDelete(currEditPath, node, targetGraph);
                        foreach (var possibleOperation in possibleOperations)
                        {
                            var tempPathToConsider = new EditPath(currEditPath.Graph, targetGraph);
                            tempPathToConsider.AddEditOperation(possibleOperation);
                            pathsToConcider.Add(tempPathToConsider);
                        }
                    }
                }
                else
                {
                    foreach (var nodeToAdd in currEditPath.TargetNodesLeftToResolve)
                    {
                        CalculatedOperation nodeAddition = new NodeAddition(currEditPath.Graph,nodeToAdd,currEditPath.EdgeAdditionsPending).GetCalculated();
                        EditPath additionPath = new EditPath(currEditPath.Graph, targetGraph);
                        additionPath.AddEditOperation(nodeAddition);
                        pathsToConcider.Add(additionPath);
                    }
                }
                var cheapestPath = pathsToConcider.First(x => x.CumelativeCostPlusHeuristic == pathsToConcider.Min(y => y.CumelativeCostPlusHeuristic));
                if (cheapestPath.HeuristicCost == 0)
                {
                    return cheapestPath;
                }
                else
                {
                    currEditPath = cheapestPath;
                }
            }
        }

        private static List<CalculatedOperation> GetPossibleSubsAndDelete(EditPath currentPath, InstructionWrapper nodeToCheck, List<InstructionWrapper> targetGraph)
        {
            var calculatedOperations = new List<CalculatedOperation>();

            foreach (var secondGraphNode in targetGraph)
            {
                var nodeSubstitution = new NodeSubstitution(currentPath.Graph, currentPath.EdgeAdditionsPending, nodeToCheck, secondGraphNode);
                calculatedOperations.Add(nodeSubstitution.GetCalculated());
            }
            var nodeDeletion = new NodeDeletion(currentPath.Graph, nodeToCheck);
            calculatedOperations.Add(nodeDeletion.GetCalculated());
            return calculatedOperations;
        }
    }
}
