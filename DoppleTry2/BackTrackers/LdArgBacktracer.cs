using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionWrappers;

namespace DoppleTry2.BackTrackers
{
    class LdArgBacktracer : SingeIndexBackTracer
    {
        public LdArgBacktracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override IEnumerable<InstructionWrapper> GetDataflowBackRelatedArgGroup(InstructionWrapper instWrapper)
        {
            List<List<InstructionWrapper>> backRelated = new List<List<InstructionWrapper>>();
            List<InstructionWrapper> stArgInst = new List<InstructionWrapper>();
            if (instWrapper.InliningProperties.Inlined)
            {
                return _SingleIndexBackSearcher.SearchBackwardsForDataflowInstrcutions(x => x is StArgInstructionWrapper &&
                                                                    ((StArgInstructionWrapper)x).ArgIndex == ((LdArgInstructionWrapper)instWrapper).ArgIndex, instWrapper);
            }
            else
            {
                return _SingleIndexBackSearcher.SafeSearchBackwardsForDataflowInstrcutions(x => x is StArgInstructionWrapper && ((StArgInstructionWrapper)x).ArgIndex == ((LdArgInstructionWrapper)instWrapper).ArgIndex, instWrapper);
            }
        }

        public override Code[] HandlesCodes => CodeGroups.LdArgCodes; 
    }
}
