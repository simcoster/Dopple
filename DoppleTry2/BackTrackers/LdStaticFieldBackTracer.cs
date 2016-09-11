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

        protected override IEnumerable<int> GetDataflowBackRelatedIndices(int instructionIndex)
        {
            var instructionsWrapper = InstructionsWrappers[instructionIndex];
            IEnumerable<int> indexes;
            if (!SafeSearchBackwardsForDataflowInstrcutions(x => x.Instruction.OpCode.Code == Code.Stsfld &&
                                                               x.Instruction.Operand == instructionsWrapper.Instruction.Operand,
                                                               instructionIndex
                                                               , out indexes))
            {
                indexes = new List<int>();
            }
            return indexes;
        }

        public override Code[] HandlesCodes => new[] {Code.Ldsfld,Code.Ldsflda};
    }
}
