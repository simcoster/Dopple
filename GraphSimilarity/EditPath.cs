using DoppleTry2.InstructionWrappers;
using GraphSimilarity.EditOperations;
using System;
using System.Collections.Generic;

namespace GraphSimilarity
{
    public class EditPath
    {
        public List<InstructionWrapper> TargetNodesLeftToResolve;
        public List<InstructionWrapper> SourceNodesLeftToResolve;
        public EditPath CloneWithEditOperation(CalculatedOperation calculatedOperation)
        {
            EditPath pathClone = Clone();
            pathClone.Path.Add(calculatedOperation);
            cumelativeCost += calculatedOperation.Cost;
            LatestOperation = calculatedOperation;
            HeuristicCost = TargetNodesLeftToResolve.Count + SourceNodesLeftToResolve.Count;
            CumelativeCostPlusHeuristic = cumelativeCost + HeuristicCost;
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

        public List<GraphEdge> EdgeAdditionsPending { get; private set; } = new List<GraphEdge>();
        public List<InstructionWrapper> Graph;
        private int cumelativeCost = 0;
        public int CumelativeCostPlusHeuristic = 0;
        public int HeuristicCost { get; private set; }
        public CalculatedOperation LatestOperation { get; private set; }
        public List<CalculatedOperation> Path { get; set; } = new List<CalculatedOperation>();
        private readonly List<InstructionWrapper> targetGraph;

        public EditPath(List<InstructionWrapper> originGraph, List<InstructionWrapper> targetGraph)
        {
            this.Graph = originGraph;
            this.targetGraph = targetGraph;
            TargetNodesLeftToResolve = new List<InstructionWrapper>(targetGraph);
            SourceNodesLeftToResolve = new List<InstructionWrapper>(originGraph);
        }
    }
}