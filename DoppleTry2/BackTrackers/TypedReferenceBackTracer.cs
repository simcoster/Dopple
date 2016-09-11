using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    class TypedReferenceBackTracer : BackTracer
    {
        public TypedReferenceBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override IEnumerable<InstructionWrapper> GetDataflowBackRelatedIndices(InstructionWrapper instWrapper)
        {
            return SearchBackwardsForDataflowInstrcutions(x => x.Instruction.OpCode.Code == Code.Mkrefany, instWrapper);
        }

        public override Code[] HandlesCodes => new[] {Code.Refanyval};
    }
}
