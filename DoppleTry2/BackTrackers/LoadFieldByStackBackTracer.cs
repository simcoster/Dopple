using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionWrappers;

namespace DoppleTry2.BackTrackers
{
    class LoadFieldByStackBackTracer : SingeIndexBackTracer
    {
        public LoadFieldByStackBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override IEnumerable<InstructionWrapper> GetDataflowBackRelatedArgGroup(InstructionWrapper instWrapper)
        {

            Func<InstructionWrapper, bool> predicate = x =>
                                  x.Instruction.OpCode.Code == Code.Stfld &&
                                  BackSearcher.HaveCommonStackPushAncestor(x, instWrapper) &&
                                  x.Instruction.Operand == instWrapper.Instruction.Operand;
            var storeFieldInsts = BackSearcher.SafeSearchBackwardsForDataflowInstrcutions(InstructionWrappers,predicate, instWrapper);
            if (storeFieldInsts.Count > 0)
            {
                return storeFieldInsts;
            }
            predicate = x =>
                x.MemoryStoreCount > 0 &&
                BackSearcher.HaveCommonStackPushAncestor(x, instWrapper);
            var storeObjInsts = BackSearcher.SafeSearchBackwardsForDataflowInstrcutions(InstructionWrappers, predicate, instWrapper);
            if (storeObjInsts.Count > 0)
            {
                return storeObjInsts;
            }
            return new InstructionWrapper[0];
        }

        public override Code[] HandlesCodes => new[] {Code.Ldfld, Code.Ldflda};

    }
}
