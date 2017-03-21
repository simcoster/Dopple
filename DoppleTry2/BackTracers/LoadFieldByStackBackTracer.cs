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
            var objectArgs = instWrapper.DataFlowBackRelated.Where(x => x.ArgIndex == 0).Select(x => x.Argument).ToArray();
            //Need to rework this, Have common stack anccesstor is not practical
            Func<InstructionNode, bool> predicate = x =>
                                  x is StoreFieldNode &&
                                  x.Instruction.Operand == instWrapper.Instruction.Operand &&
                                  x.DataFlowBackRelated.Where(y => y.ArgIndex == 0).Select(y => y.Argument).SequenceEqual(objectArgs);
            return SingleIndexBackSearcher.SafeSearchBackwardsForDataflowInstrcutions(predicate, instWrapper);
        }

        public override Code[] HandlesCodes => new[] {Code.Ldfld, Code.Ldflda};

    }
}
