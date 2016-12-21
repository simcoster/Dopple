using DoppleTry2.InstructionWrappers;
using GraphSimilarity.EditOperations;
using System.Collections.Generic;

namespace GraphSimilarity
{
    internal class EditPath
    {
        public void AddEditOperation(CalculatedOperation calculatedOperation)
        {
            Path.Add(calculatedOperation);
            CumelativeCost += calculatedOperation.Cost;
            Graph = calculatedOperation.EditedGraph;
            HeuristicCost = HeuristicDistanceCalc.HeuristicNodeDistance(Graph, targetGraph);
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
        }
    }
}