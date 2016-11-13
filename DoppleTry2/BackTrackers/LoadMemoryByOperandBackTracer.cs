using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionWrappers;

namespace DoppleTry2.BackTrackers
{
    class LoadMemoryByOperandBackTracer : SingeIndexBackTracer
    {
        public LoadMemoryByOperandBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override IEnumerable<InstructionWrapper> GetDataflowBackRelatedArgGroup(InstructionWrapper instWrapper)
        {
            var storeIndex =
            BackSearcher.SearchBackwardsForDataflowInstrcutions(InstructionWrappers, x => x.MemoryStoreCount > 0 &&
                                                        instWrapper.Instruction.Operand ==
                                                        x.Instruction.Operand, instWrapper);
            return storeIndex;
        }

        public override Code[] HandlesCodes => new[]
        {
            Code.Ldind_I1, Code.Ldind_U1, Code.Ldind_I2,
            Code.Ldind_U2, Code.Ldind_I4, Code.Ldind_U4,
            Code.Ldind_I8, Code.Ldind_I, Code.Ldind_R4,
            Code.Ldind_R8, Code.Ldind_Ref};

    }
}
