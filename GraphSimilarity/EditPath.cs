using Dopple.InstructionNodes;
using GraphSimilarity.EditOperations;
using System;
using System.Collections.Generic;

namespace GraphSimilarity
{
    public class EditPath : IComparable
    {
        private static int index = 0;
        public int MyIndex { get; private set; }
        public List<InstructionNode> TargetNodesLeftToResolve;
        public List<InstructionNode> SourceNodesLeftToResolve;
        public EditPath CloneWithEditOperation(CalculatedOperation calculatedOperation)
        {
            EditPath pathClone = Clone();
            pathClone.Path.Add(calculatedOperation);
            pathClone.cumelativeCost += calculatedOperation.Cost;
            pathClone.LatestOperation = calculatedOperation;
            foreach(var sourceNodeToRemove in calculatedOperation.DeletedNodes)
            {
                pathClone.SourceNodesLeftToResolve.Remove(sourceNodeToRemove);
            }
            foreach (var targetNodeToRemove in calculatedOperation.AddedNodes)
            {
                pathClone.TargetNodesLeftToResolve.Remove(targetNodeToRemove);
            }
            pathClone.HeuristicCost = pathClone.TargetNodesLeftToResolve.Count + pathClone.SourceNodesLeftToResolve.Count;
            pathClone.CumelativeCostPlusHeuristic = pathClone.cumelativeCost + pathClone.HeuristicCost;
            return pathClone;
        }

        private EditPath Clone()
        {
            var editPathClone = new EditPath(Graph, targetGraph);
            editPathClone.EdgeAdditionsPending = new List<GraphEdge>(EdgeAdditionsPending);
            editPathClone.Path = new List<CalculatedOperation>(Path);
            editPathClone.cumelativeCost = cumelativeCost;
            editPathClone.SourceNodesLeftToResolve = new List<InstructionNode>(SourceNodesLeftToResolve);
            editPathClone.TargetNodesLeftToResolve= new List<InstructionNode>(TargetNodesLeftToResolve);
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
        public List<InstructionNode> Graph;
        private int cumelativeCost = 0;
        public int CumelativeCostPlusHeuristic = 0;
        public int HeuristicCost { get; private set; }
        public CalculatedOperation LatestOperation { get; private set; } = new CalculatedOperation();
        public List<CalculatedOperation> Path { get; set; } = new List<CalculatedOperation>();
        private readonly List<InstructionNode> targetGraph;

        public EditPath(List<InstructionNode> originGraph, List<InstructionNode> targetGraph)
        {
            this.Graph = originGraph;
            this.targetGraph = targetGraph;
            TargetNodesLeftToResolve = new List<InstructionNode>(targetGraph);
            SourceNodesLeftToResolve = new List<InstructionNode>(originGraph);
            MyIndex = index;
            index++;
        }
    }
}