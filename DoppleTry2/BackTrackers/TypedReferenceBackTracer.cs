using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionWrappers;

namespace DoppleTry2.BackTrackers
{
    class TypedReferenceBackTracer : SingeIndexBackTracer
    {
        public TypedReferenceBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override IEnumerable<InstructionWrapper> GetDataflowBackRelatedArgGroup(InstructionWrapper instWrapper)
        {
            return _SingleIndexBackSearcher.SearchBackwardsForDataflowInstrcutions(x => x.Instruction.OpCode.Code == Code.Mkrefany, instWrapper);
        }


        public override Code[] HandlesCodes => new[] {Code.Refanyval};
    }
}
