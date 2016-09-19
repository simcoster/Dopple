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
        protected override IEnumerable<InstructionWrapper> GetDataflowBackRelatedIndices(InstructionWrapper instWrapper)
        {

            Func<InstructionWrapper, bool> predicate = x => 
                                  x.Instruction.OpCode.Code == Code.Stfld &&
                                  HaveCommonStackPushAncestor(x, instWrapper) &&
                                  x.Instruction.Operand == instWrapper.Instruction.Operand;
            var storeFieldInsts = SafeSearchBackwardsForDataflowInstrcutions(predicate, instWrapper);
            if (storeFieldInsts.Count > 0)
            {
                return storeFieldInsts;
            }
            predicate = x =>
                x.MemoryStoreCount > 0&&
                HaveCommonStackPushAncestor(x, instWrapper);
            var storeObjInsts = SafeSearchBackwardsForDataflowInstrcutions(predicate, instWrapper);
            if (storeObjInsts.Count > 0)
            {
                return storeObjInsts;
            }
            return new InstructionWrapper[0];
        }

        public override Code[] HandlesCodes => new[] {Code.Ldfld, Code.Ldflda,};

    }
}
