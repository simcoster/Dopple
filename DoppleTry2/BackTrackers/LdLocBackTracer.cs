using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionNodes;

namespace DoppleTry2.BackTrackers
{
    public class LdLocBackTracer : SingeIndexBackTracer
    {
        protected override IEnumerable<InstructionNode> GetDataflowBackRelatedArgGroup(InstructionNode instNode)
        {
            LocationLoadInstructionNode ldInstWrapper = (LocationLoadInstructionNode)instNode;
            return _SingleIndexBackSearcher.SearchBackwardsForDataflowInstrcutions(x => x is LocationStoreInstructionNode && 
                                    ((LocationStoreInstructionNode)x).LocIndex == ldInstWrapper.LocIndex, instNode);
        }

        public override Code[] HandlesCodes => CodeGroups.LdLocCodes;

        public LdLocBackTracer(List<InstructionNode> instructionNodes) : base(instructionNodes)
        {
        }
    }
}