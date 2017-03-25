using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using Dopple.InstructionNodes;

namespace Dopple.BackTracers
{
    class LdArgBacktracer : BackTracer
    {
        public override void BackTraceDataFlow(InstructionNode currentInst)
        {
            if (currentInst.InliningProperties.Inlined)
            {
                InlineableCallNode inlinedCall = currentInst.InliningProperties.CallNode;
                var argSuppliers = inlinedCall.DataFlowBackRelated.Where(x => x.ArgIndex == ((LdArgInstructionNode) currentInst).ArgIndex).Select(x => x.Argument).ToList();
                currentInst.DataFlowBackRelated.AddTwoWaySingleIndex(argSuppliers);
            }
            else
            {
                //TODO, need to implement STARG as well (even though it's not that commonly used)
            }
        }

        public override Code[] HandlesCodes => CodeGroups.LdArgCodes; 
    }
}
