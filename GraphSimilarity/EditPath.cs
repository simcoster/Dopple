using DoppleTry2.InstructionWrappers;
using GraphSimilarity.EditOperations;
using System.Collections.Generic;

namespace GraphSimilarity
{
    public class EditPath
    {
        public List<InstructionWrapper> TargetNodesLeftToResolve;
        public List<InstructionWrapper> SourceNodesLeftToResolve;
        public void AddEditOperation(CalculatedOperation calculatedOperation)
        {
            Path.Add(calculatedOperation);
            CumelativeCost += calculatedOperation.Cost;
            Graph = calculatedOperation.EditedGraph;
            HeuristicCost = TargetNodesLeftToResolve.Count + SourceNodesLeftToResolve.Count;
            CumelativeCostPlusHeuristic = CumelativeCost + HeuristicCost;
        }
        public List<GraphEdge> EdgeAdditionsPending { get; private set; } = new List<GraphEdge>();
        public List<InstructionWrapper> Graph;
        private int CumelativeCost = 0;
        public int CumelativeCostPlusHeuristic = 0;
        public List<CalculatedOperation> ReadOnlyPath { get; private set; } = new List<CalculatedOperation>();
        public int HeuristicCost { get; private set; }

        private readonly List<CalculatedOperation> Path = new List<CalculatedOperation>();
        private readonly List<InstructionWrapper> targetGraph;

        public EditPath(List<InstructionWrapper> graphToClone, List<InstructionWrapper> targetGraph)
        {
            this.Graph = new List<InstructionWrapper>(graphToClone);
            this.targetGraph = targetGraph;
            TargetNodesLeftToResolve = new List<InstructionWrapper>(targetGraph);
            SourceNodesLeftToResolve = new List<InstructionWrapper>(graphToClone);
        }
    }
}