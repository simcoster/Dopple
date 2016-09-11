using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    class LoadArrayBackTracer : BackTracer
    {
        private readonly Code[] _stArrayCodes = new[] {Code.Newarr}.Concat(LdArgBacktracer.LdArgCodes).ToArray();

        public LoadArrayBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }


        protected override IEnumerable<InstructionWrapper> GetDataflowBackRelatedIndices(InstructionWrapper instWrapper)
        {
            return SearchBackwardsForDataflowInstrcutions(x => _stArrayCodes.Contains(x.Instruction.OpCode.Code)
                                                               && HaveCommonStackPushAncestor(x, instWrapper),instWrapper);
        }

        public override Code[] HandlesCodes => new[] {Code.Ldlen,};
    }
}
