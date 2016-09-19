using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    class LdStaticFieldBackTracer : BackTracer
    {
        public LdStaticFieldBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }


        protected override IEnumerable<InstructionWrapper> GetDataflowBackRelatedIndices(InstructionWrapper instWrapper)
        {

            return SafeSearchBackwardsForDataflowInstrcutions(x => x.Instruction.OpCode.Code == Code.Stsfld &&
                                                                   x.Instruction.Operand ==
                                                                   instWrapper.Instruction.Operand, instWrapper);
        }

        public override Code[] HandlesCodes => new[] {Code.Ldsfld,Code.Ldsflda};
    }
}
