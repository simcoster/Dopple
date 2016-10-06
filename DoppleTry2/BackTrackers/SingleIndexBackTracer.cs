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
        }

        protected override IEnumerable<IEnumerable<InstructionWrapper>> GetDataflowBackRelated(InstructionWrapper instWrapper)
        {
            return new List<List<InstructionWrapper>>() { { GetDataflowBackRelatedArgGroup(instWrapper).ToList() } };
        }

        protected abstract IEnumerable<InstructionWrapper> GetDataflowBackRelatedArgGroup(InstructionWrapper instWrapper);
    }
}
