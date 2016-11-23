using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionWrappers;

namespace DoppleTry2.BackTrackers
{
    public class LdLocBackTracer : SingeIndexBackTracer
    {
        protected override IEnumerable<InstructionWrapper> GetDataflowBackRelatedArgGroup(InstructionWrapper instWrapper)
        {
            LocationLoadInstructionWrapper ldInstWrapper = (LocationLoadInstructionWrapper)instWrapper;
            return _SingleIndexBackSearcher.SearchBackwardsForDataflowInstrcutions(x => x is LocationStoreInstructionWrapper && 
                                    ((LocationStoreInstructionWrapper)x).LocIndex == ldInstWrapper.LocIndex, instWrapper);
        }

        public override Code[] HandlesCodes => CodeGroups.LdLocCodes;

        public LdLocBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }
    }
}