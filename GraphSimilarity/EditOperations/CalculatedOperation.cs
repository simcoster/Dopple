using DoppleTry2.InstructionWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarity.EditOperations
{
    public class CalculatedOperation
    {
        public NodeEditOperation NodeOperation { get;  set; }
        public List<EdgeEditOperation> EdgeOperations { get; set; } = new List<EdgeEditOperation>();
        public List<InstructionWrapper> EditedGraph { get;  set; }
        public List<InstructionWrapper> DeletedNodes { get; set; }
        public List<InstructionWrapper> AddedNodes { get; set; }
        public int Cost { get; set; }
        public string Description
        {
            get {
                if (NodeOperation == null)
                {
                    return "";
                }
                string edgeOperationsDesc = "";
                foreach(var edgeOperation in EdgeOperations)
                {
                    edgeOperationsDesc += edgeOperation.Description + " , ";
                }
                return NodeOperation.Description + " , " + edgeOperationsDesc;
            }
        }
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
