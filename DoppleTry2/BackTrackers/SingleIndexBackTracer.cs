using DoppleTry2.InstructionWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.BackTrackers
{
    public abstract class SingeIndexBackTracer : BackTracer
    {
        public SingeIndexBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
            _SingleIndexBackSearcher = new SingleIndexBackSearcher(instructionsWrappers);
        }
        protected SingleIndexBackSearcher _SingleIndexBackSearcher;

        protected IEnumerable<IEnumerable<InstructionWrapper>> GetDataflowBackRelated(InstructionWrapper instWrapper)
        {
            return new List<List<InstructionWrapper>>() { { GetDataflowBackRelatedArgGroup(instWrapper).ToList() } };
        }
        public override void AddBackDataflowConnections(InstructionWrapper currentInst)
        {
            if (currentInst.DoneBackTracers.Contains(GetType()))
            {
                return;
            }
            var backRelatedInsts = GetDataflowBackRelated(currentInst);

            foreach (var backRelatedGroup in backRelatedInsts)
            {
                currentInst.BackDataFlowRelated.AddWithNewIndex(backRelatedGroup);
                foreach (var backInst in backRelatedGroup)
                {
                    backInst.ForwardDataFlowRelated.AddWithNewIndex(currentInst);
                }
            }
            currentInst.DoneBackTracers.Add(GetType());
        }

        protected abstract IEnumerable<InstructionWrapper> GetDataflowBackRelatedArgGroup(InstructionWrapper instWrapper);

       
    }
}
