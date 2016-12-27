using System;
using DoppleTry2.InstructionNodes;
using GraphSimilarity.EditOperations;
using System.Collections.Generic;
using System.Linq;

namespace GraphSimilarity
{
    public abstract class NodeEditOperation : EditOperation
    {
        public NodeEditOperation(List<InstructionNode> graph, InstructionNode node) : base(graph)
        {
            Node = node;
        }

        public InstructionNode Node { get; private set; }
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

        internal abstract List<InstructionNode> GetAddeddNodes();
        internal abstract List<InstructionNode> GetDeletedNodes();
        internal abstract List<EdgeEditOperation> GetEdgeOperations();
    }
}