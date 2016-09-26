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
            return SearchBackwardsForDataflowInstrcutions(x => x.LocIndex == instWrapper.LocIndex && _storingCodes.Contains(x.Instruction.OpCode.Code),
                instWrapper);
        }

        private readonly Code[] _storingCodes = {Code.Stloc, Code.Stloc_0, Code.Stloc_1, Code.Stloc_2, Code.Stloc_3, Code.Stloc_S};
        public override Code[] HandlesCodes => new []{Code.Ldloc_0, Code.Ldloc_1, Code.Ldloc_2, Code.Ldloc_3, Code.Ldloc, Code.Ldloc_S, Code.Ldloca_S, Code.Ldloca, };

        public LdLocBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }
    }
}