using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionNodes;

namespace DoppleTry2.BackTrackers
{
    class LdStaticFieldBackTracer : SingeIndexBackTracer
    {
        public LdStaticFieldBackTracer(List<InstructionNode> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override IEnumerable<InstructionNode> GetDataflowBackRelatedArgGroup(InstructionNode instWrapper)
        {
            return _SingleIndexBackSearcher.SafeSearchBackwardsForDataflowInstrcutions(x => x.Instruction.OpCode.Code == Code.Stsfld &&
                                                                  x.Instruction.Operand ==
                                                                  instWrapper.Instruction.Operand, instWrapper);
        }

        public override Code[] HandlesCodes => new[] {Code.Ldsfld,Code.Ldsflda};
    }
}
