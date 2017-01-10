using DoppleTry2.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.BackTrackers
{
    public abstract class SingeIndexBackTracer : BackTracer
    {
        public SingeIndexBackTracer(List<InstructionNode> instructionsWrappers) : base(instructionsWrappers)
        {
            _SingleIndexBackSearcher = new SingleIndexBackSearcher(instructionsWrappers);
        }
        protected SingleIndexBackSearcher _SingleIndexBackSearcher;

        protected IEnumerable<IEnumerable<InstructionNode>> GetDataflowBackRelated(InstructionNode instWrapper)
        {
            return new List<List<InstructionNode>>() { { GetDataflowBackRelatedArgGroup(instWrapper).ToList() } };
        }
        protected override void InnerAddBackDataflowConnections(InstructionNode currentInst)
        {        
            IEnumerable<IEnumerable<InstructionNode>> backRelatedGroups = GetDataflowBackRelated(currentInst);

            foreach (var backRelatedGroup in backRelatedGroups)
            {
                currentInst.DataFlowBackRelated.AddTwoWaySingleIndex(backRelatedGroup);
            }
        }

        protected abstract IEnumerable<InstructionNode> GetDataflowBackRelatedArgGroup(InstructionNode instWrapper);

       
    }
}
