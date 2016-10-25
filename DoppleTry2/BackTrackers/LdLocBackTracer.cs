using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    public class LdLocBackTracer : SingeIndexBackTracer
    {
        protected override IEnumerable<InstructionWrapper> GetDataflowBackRelatedArgGroup(InstructionWrapper instWrapper)
        {

            return SearchBackwardsForDataflowInstrcutions(x => x.LocIndex == instWrapper.LocIndex && CodeGroups.LocStoreCodes.Contains(x.Instruction.OpCode.Code),
                instWrapper);
        }

        public override Code[] HandlesCodes => CodeGroups.LocLoadCodes;

        public LdLocBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }
    }
}