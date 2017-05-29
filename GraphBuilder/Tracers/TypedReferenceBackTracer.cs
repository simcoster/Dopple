using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using Dopple.InstructionNodes;

namespace Dopple.BackTracers
{
    class TypedReferenceBackTracer : DataflowBacktracer
    {
        protected override IEnumerable<InstructionNode> GetDataflowBackRelatedArgGroup(InstructionNode instWrapper)
        {
            bool allPathsFoundAMatch;
            return SingleIndexBackSearcher.SafeSearchBackwardsForDataflowInstrcutions(x => x.Instruction.OpCode.Code == Code.Mkrefany, instWrapper, out allPathsFoundAMatch);
        }


        public override Code[] HandlesCodes => new[] {Code.Refanyval};
    }
}
