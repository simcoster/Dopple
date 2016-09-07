using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    class LoadFieldByStackBackTracer : BackTracer
    {
        public LoadFieldByStackBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override IEnumerable<int> GetDataflowBackRelatedIndices(int instructionIndex)
        {
            var currInst = InstructionsWrappers[instructionIndex];
            Func<InstructionWrapper, bool> storeFld = (x => x.Instruction.OpCode.Code == Code.Stfld &&
                                                            x.Instruction.Operand == currInst.Instruction.Operand &&
                                                            HaveCommonStackPushAncestor(currInst, x));

            TrySearchBackwardsForDataflowInstrcutions()
        }

        private bool HaveCommonStackPushAncestor(InstructionWrapper currInst, InstructionWrapper instructionWrapper)
        {
            SearchBackwardsForDataflowInstrcutions(x => x.StackPushCount)
        }


        public override Code[] HandlesCodes { get; }

        public override Run
    }
}
