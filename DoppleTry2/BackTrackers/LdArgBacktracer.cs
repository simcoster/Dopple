using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionWrappers;

namespace DoppleTry2.BackTrackers
{
    class LdArgBacktracer :  BackTracer
    {
        public LdArgBacktracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override IEnumerable<IEnumerable<InstructionWrapper>> GetDataflowBackRelated(InstructionWrapper instWrapper)
        {
            List < List < InstructionWrapper >> backRelated = new List<List<InstructionWrapper>>();
            List<InstructionWrapper> stArgInst = new List<InstructionWrapper>();
            if (instWrapper.InliningProperties.Inlined)
            {
                backRelated.Add(BackSearcher.SearchBackwardsForDataflowInstrcutions(InstructionWrappers,
                                                                    x => x is StArgInstructionWrapper &&
                                                                    ((StArgInstructionWrapper)x).ArgIndex == ((LdArgInstructionWrapper)instWrapper).ArgIndex, instWrapper));
            }
            else
            {
                BackSearcher.SafeSearchBackwardsForDataflowInstrcutions(InstructionWrappers, x => x is StArgInstructionWrapper &&
                                                                                                      ((StArgInstructionWrapper)x).ArgIndex == ((LdArgInstructionWrapper)instWrapper).ArgIndex, instWrapper);
                if (stArgInst.Count != 0)
                {
                    backRelated.Add(stArgInst);
                }
            }
            return backRelated;
        }

        public override Code[] HandlesCodes => CodeGroups.LdArgCodes;

       
    }
}
