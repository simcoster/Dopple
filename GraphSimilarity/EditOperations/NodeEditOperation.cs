using System;
using DoppleTry2.InstructionWrappers;
using GraphSimilarity.EditOperations;
using System.Collections.Generic;
using System.Linq;

namespace GraphSimilarity
{
    internal abstract class NodeEditOperation : EditOperation
    {
        public NodeEditOperation(List<InstructionWrapper> graph, InstructionWrapper node) : base(graph)
        {
            Node = node;
        }

        public InstructionWrapper Node { get; private set; }
        public CalculatedOperation GetCalculated()
        {
            var computedNodeOperation = new CalculatedOperation();
            computedNodeOperation.NodeOperation = this;
            computedNodeOperation.DeletedNodes = GetDeletedNodes();
            computedNodeOperation.AddedNodes = GetAddeddNodes();
            computedNodeOperation.EdgeOperations = GetEdgeOperations();
            computedNodeOperation.Cost = computedNodeOperation.NodeOperation.Cost + computedNodeOperation.EdgeOperations.Sum(x => x.Cost);
            return computedNodeOperation;
        }

        internal abstract List<InstructionWrapper> GetAddeddNodes();
        internal abstract List<InstructionWrapper> GetDeletedNodes();
        internal abstract List<EdgeEditOperation> GetEdgeOperations();
    }
}