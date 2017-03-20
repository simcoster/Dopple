using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using Dopple.InstructionNodes;

namespace Dopple.BackTracers
{
    class LoadFieldByStackBackTracer : SingeIndexBackTracer
    {
        protected override IEnumerable<InstructionNode> GetDataflowBackRelatedArgGroup(InstructionNode instWrapper)
        {

            //Need to rework this, Have common stack anccesstor is not practical
            Func<InstructionNode, bool> predicate = x =>
                                  x.Instruction.OpCode.Code == Code.Stfld &&
                                  x.Instruction.Operand == instWrapper.Instruction.Operand &&
                                  BackSearcher.HaveCommonStackPushAncestor(x, instWrapper);
            var storeFieldInsts = SingleIndexBackSearcher.SafeSearchBackwardsForDataflowInstrcutions(predicate, instWrapper);
            if (storeFieldInsts.Count > 0)
            {
                return storeFieldInsts;
            }
            predicate = x =>
                x.MemoryStoreCount > 0 &&
                BackSearcher.HaveCommonStackPushAncestor(x, instWrapper);
            var storeObjInsts = SingleIndexBackSearcher.SafeSearchBackwardsForDataflowInstrcutions(predicate, instWrapper);
            if (storeObjInsts.Count > 0)
            {
                return storeObjInsts;
            }
            return new InstructionNode[0];
        }

        public override Code[] HandlesCodes => new[] {Code.Ldfld, Code.Ldflda};

    }
}
