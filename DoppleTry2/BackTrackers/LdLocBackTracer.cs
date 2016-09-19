using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    public class LdLocBackTracer : BackTracer
    {

        protected override IEnumerable<InstructionWrapper> GetDataflowBackRelatedIndices(InstructionWrapper instWrapper)
        {
            return SearchBackwardsForDataflowInstrcutions(x => x.LocIndex == instWrapper.LocIndex,instWrapper);
        }

        public override Code[] HandlesCodes => new []{Code.Ldloc_0, Code.Ldloc_1, Code.Ldloc_2, Code.Ldloc, Code.Ldloc_S, Code.Ldloca_S, Code.Ldloca, };

        public LdLocBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }
    }
}