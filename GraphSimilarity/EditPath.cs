using DoppleTry2.InstructionWrappers;
using GraphSimilarity.EditOperations;
using System;
using System.Collections.Generic;

namespace GraphSimilarity
{
    public class EditPath : IComparable
    {
        private static int index = 0;
        public int MyIndex { get; private set; }
        public List<InstructionWrapper> TargetNodesLeftToResolve;
        public List<InstructionWrapper> SourceNodesLeftToResolve;
        public EditPath CloneWithEditOperation(CalculatedOperation calculatedOperation)
        {
            EditPath pathClone = Clone();
            pathClone.Path.Add(calculatedOperation);
            pathClone.cumelativeCost += calculatedOperation.Cost;
            pathClone.LatestOperation = calculatedOperation;
            pathClone.HeuristicCost = TargetNodesLeftToResolve.Count + SourceNodesLeftToResolve.Count;
            pathClone.CumelativeCostPlusHeuristic = pathClone.cumelativeCost + pathClone.HeuristicCost;
            return pathClone;
        }

        private EditPath Clone()
        {
            var editPathClone = new EditPath(Graph, targetGraph);
            editPathClone.EdgeAdditionsPending = new List<GraphEdge>(EdgeAdditionsPending);
            editPathClone.Path = new List<CalculatedOperation>(Path);
            editPathClone.cumelativeCost = cumelativeCost;
            return editPathClone;
        }

        public int CompareTo(object obj)
        {
            if (obj is EditPath)
            {
                return CumelativeCostPlusHeuristic.CompareTo(((EditPath) obj).CumelativeCostPlusHeuristic);
                
            }
            else
            {
                throw new Exception("other object is not an EditPath");
            }
        }

        public List<GraphEdge> EdgeAdditionsPending { get; private set; } = new List<GraphEdge>();
        public List<InstructionWrapper> Graph;
        private int cumelativeCost = 0;
        public int CumelativeCostPlusHeuristic = 0;
        public int HeuristicCost { get; private set; }
        public CalculatedOperation LatestOperation { get; private set; } = new CalculatedOperation();
        public List<CalculatedOperation> Path { get; set; } = new List<CalculatedOperation>();
        private readonly List<InstructionWrapper> targetGraph;

        public EditPath(List<InstructionWrapper> originGraph, List<InstructionWrapper> targetGraph)
        {
            this.Graph = originGraph;
            this.targetGraph = targetGraph;
            TargetNodesLeftToResolve = new List<InstructionWrapper>(targetGraph);
            SourceNodesLeftToResolve = new List<InstructionWrapper>(originGraph);
            MyIndex = index;
            index++;
        }
    }
}