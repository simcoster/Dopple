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
            CurrentGraphState = calculatedOperation.EditedGraph;
        }
        public List<InstructionWrapper> CurrentGraphState;
        public int CumelativeCost = 0;
        public List<CalculatedOperation> ReadOnlyPath { get; private set; } = new List<CalculatedOperation>();
        private readonly List<CalculatedOperation> Path = new List<CalculatedOperation>();

    }
}