using DoppleTry2.InstructionWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarity.EditOperations
{
    internal class CalculatedOperation
    {
        public NodeEditOperation NodeOperation { get;  set; }
        public List<EdgeEditOperation> EdgeOperations { get;  set; }
        public List<InstructionWrapper> EditedGraph { get;  set; }
        public List<InstructionWrapper> DeletedNodes { get; set; }
        public List<InstructionWrapper> AddedNodes { get; set; }
        public int Cost { get; set; }
        public void Commit()
        {
            NodeOperation.Commit();
            foreach(var edgeEdit in EdgeOperations)
            {
                edgeEdit.Commit();
            }
        }
    }
}
