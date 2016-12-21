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
            IEnumerable<InstructionWrapper> targetGraphNodesToResolve = new List<InstructionWrapper>(targetGraph);
            IEnumerable<InstructionWrapper> sourceGraphNodesToResolve = new List<InstructionWrapper>(sourceGraph);
            var pathsToConcider = new List<EditPath>();
            var currGraph = sourceGraph;
            while (true)
            {
                IEnumerable<CalculatedOperation> possibleOperations = currGraph.SelectMany(x => GetPossibleSubsAndDelete(sourceGraph, x, targetGraph));
                foreach (var possibleOperation in possibleOperations)
                {
                    var tempPathToConsider = new EditPath(currGraph, targetGraph);
                    tempPathToConsider.AddEditOperation(possibleOperation);
                    pathsToConcider.Add(tempPathToConsider);
                }
                var cheapestPath = pathsToConcider.First(x => x.CumelativeCostPlusHeuristic == pathsToConcider.Min(y => y.CumelativeCostPlusHeuristic));               
                if (cheapestPath.HeuristicCost == 0)
                {
                    return cheapestPath;
                }
                else
                {
                    currGraph = cheapestPath.Graph;
                }
            }
            return 0;
        }

        private static List<CalculatedOperation> GetPossibleSubsAndDelete(List<InstructionWrapper> graphToChange, InstructionWrapper nodeToCheck, List<InstructionWrapper> targetGraph)
        {
            var calculatedOperations = new List<CalculatedOperation>();

            foreach (var secondGraphNode in targetGraph)
            {
                var startWithReplacePath = new EditPath(graphToChange);
                var nodeSubstitution = new NodeSubstitution(startWithReplacePath.Graph, startWithReplacePath.EdgeAdditionsPending, nodeToCheck, secondGraphNode);
                calculatedOperations.Add(nodeSubstitution.GetCalculated());
            }
            var startWithDeletePath = new EditPath(graphToChange);
            var nodeDeletion = new NodeDeletion(startWithDeletePath.Graph, nodeToCheck);
            startWithDeletePath.AddEditOperation(nodeDeletion.GetCalculated());
            return calculatedOperations;
        }
    }
}
