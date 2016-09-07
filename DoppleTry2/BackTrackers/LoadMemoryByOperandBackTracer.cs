using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    class LoadMemoryByOperandBackTracer : BackTracer
    {
        public LoadMemoryByOperandBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override IEnumerable<int> GetDataflowBackRelatedIndices(int instructionIndex)
        {
            var storeIndex =
             SearchBackwardsForDataflowInstrcutions(x => x.MemoryStoreCount > 0 &&
                                                         InstructionsWrappers[instructionIndex].Instruction.Operand ==
                                                         x.Instruction.Operand, instructionIndex);
            return storeIndex;
        }

        public override Code[] HandlesCodes => new[]
        {
            Code.Ldind_I1, Code.Ldind_U1, Code.Ldind_I2,
            Code.Ldind_U2, Code.Ldind_I4, Code.Ldind_U4,
            Code.Ldind_I8, Code.Ldind_I, Code.Ldind_R4,
            Code.Ldind_R8, Code.Ldind_Ref        };

    }
}
