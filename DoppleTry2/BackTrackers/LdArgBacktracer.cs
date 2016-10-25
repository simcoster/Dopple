using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

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
            Code[] relevantCodes = { Code.Starg, Code.Starg_S };
            List<InstructionWrapper> stArgInst = new List<InstructionWrapper>();
            if (instWrapper.Inlined)
            {
                backRelated.Add(BackSearcher.SearchBackwardsForDataflowInstrcutions(InstructionWrappers,
                                                                    x => relevantCodes.Contains(x.Instruction.OpCode.Code) &&
                                                                    x.ArgIndex == instWrapper.ArgIndex, instWrapper));
            }
            else
            {
                BackSearcher.SafeSearchBackwardsForDataflowInstrcutions(InstructionWrappers, x =>
                                                                    relevantCodes.Contains(x.Instruction.OpCode.Code) &&
                                                                    x.ArgIndex == instWrapper.ArgIndex, instWrapper);
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
