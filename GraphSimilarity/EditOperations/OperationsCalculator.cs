using DoppleTry2.InstructionWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarity.EditOperations
{
    class EdgeOperationsCalculator
    {
        private List<InstructionWrapper> _SourceGraph;
        public EdgeOperationsCalculator(List<InstructionWrapper> sourceGraph)
        {
            _SourceGraph = sourceGraph;
        }
    }

    class NodeOperationWithRelatedEdges
    {
        public NodeEditOperation NodeOperation { get; set; }
        public List<EditOperation> EdgeOperations { get; set; }
        public int TotalCost { get; set; }
        public NodeOperationWithRelatedEdges(NodeEditOperation nodeOperation, List<EditOperation> EdgeOperations)
        {
            NodeOperation = nodeOperation;

        }
    }
}
