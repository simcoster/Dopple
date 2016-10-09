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

        public override Code[] HandlesCodes => LdArgCodes;

        public static Code[] LdArgCodes = {
            Code.Ldarg, Code.Ldarg_0, Code.Ldarg_1, Code.Ldarg_2, Code.Ldarg_3, Code.Ldarg_S,
            Code.Ldarga, Code.Ldarga_S
        };
    }
}
