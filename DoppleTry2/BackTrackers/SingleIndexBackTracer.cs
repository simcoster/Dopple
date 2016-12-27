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
        public override void AddBackDataflowConnections(InstructionNode currentInst)
        {
            if (currentInst.DoneBackTracers.Contains(GetType()))
            {
                return;
            }
            IEnumerable<IEnumerable<InstructionNode>> backRelatedGroups = GetDataflowBackRelated(currentInst);

            foreach (var backRelatedGroup in backRelatedGroups)
            {
                currentInst.DataFlowBackRelated.AddWithNewIndex(backRelatedGroup);
            }
            currentInst.DoneBackTracers.Add(GetType());
        }

        protected abstract IEnumerable<InstructionNode> GetDataflowBackRelatedArgGroup(InstructionNode instWrapper);

       
    }
}
