using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    class LdStaticFieldBackTracer : SingeIndexBackTracer
    {
        public LdStaticFieldBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override IEnumerable<InstructionWrapper> GetDataflowBackRelatedArgGroup(InstructionWrapper instWrapper)
        {
            return BackSearcher.SafeSearchBackwardsForDataflowInstrcutions(InstructionWrappers, x => x.Instruction.OpCode.Code == Code.Stsfld &&
                                                                  x.Instruction.Operand ==
                                                                  instWrapper.Instruction.Operand, instWrapper);
        }

        public override Code[] HandlesCodes => new[] {Code.Ldsfld,Code.Ldsflda};
    }
}
