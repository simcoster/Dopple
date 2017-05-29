using Dopple.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.BackTracers
{
    public abstract class DataflowBacktracer : BackTracer
    {
        protected IEnumerable<IEnumerable<InstructionNode>> GetDataflowBackRelated(InstructionNode instNode)
        {
            return new List<List<InstructionNode>>() { { GetDataflowBackRelatedArgGroup(instNode).ToList() } };
        }
        public override void BackTraceDataFlow(InstructionNode currentInst)
        {        
            IEnumerable<IEnumerable<InstructionNode>> backRelatedGroups = GetDataflowBackRelated(currentInst);

            foreach (var backRelatedGroup in backRelatedGroups)
            {
                currentInst.DataFlowBackRelated.AddTwoWaySingleIndex(backRelatedGroup);
            }
        }

        protected abstract IEnumerable<InstructionNode> GetDataflowBackRelatedArgGroup(InstructionNode instructionNode);

       
    }
}
