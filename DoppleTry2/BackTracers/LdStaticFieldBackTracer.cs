using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using Dopple.InstructionNodes;

namespace Dopple.BackTracers
{
    class LdStaticFieldBackTracer : SingeIndexBackTracer
    {
        protected override IEnumerable<InstructionNode> GetDataflowBackRelatedArgGroup(InstructionNode instWrapper)
        {
            return SingleIndexBackSearcher.SafeSearchBackwardsForDataflowInstrcutions(x => x.Instruction.OpCode.Code == Code.Stsfld &&
                                                                  x.Instruction.Operand ==
                                                                  instWrapper.Instruction.Operand, instWrapper);
        }

        public override Code[] HandlesCodes => new[] {Code.Ldsfld,Code.Ldsflda};
    }
}
