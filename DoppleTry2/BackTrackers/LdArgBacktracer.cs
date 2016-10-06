using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    class LdArgBacktracer : SingeIndexBackTracer
    {
        public LdArgBacktracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override IEnumerable<InstructionWrapper> GetDataflowBackRelatedArgGroup(InstructionWrapper instWrapper)
        {
            Code[] relevantCodes = { Code.Starg, Code.Starg_S };
            var stArgInst = SafeSearchBackwardsForDataflowInstrcutions(
                x => relevantCodes.Contains(x.Instruction.OpCode.Code) &&
                x.ArgIndex == instWrapper.ArgIndex, instWrapper);
            if (stArgInst.Count == 0)
            {
                return new InstructionWrapper[0];
            }
            return stArgInst;
        }

        public override Code[] HandlesCodes => LdArgCodes;

        public static Code[] LdArgCodes = {
            Code.Ldarg, Code.Ldarg_0, Code.Ldarg_1, Code.Ldarg_2, Code.Ldarg_3, Code.Ldarg_S,
            Code.Ldarga, Code.Ldarga_S
        };
    }
}
