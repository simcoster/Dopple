using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;
using Mono.Cecil.Cil;

namespace Dopple.BackTracers
{
    class RetBackTracer : BackTracer
    {
        public override Code[] HandlesCodes
        {
            get
            {
                return new[] { Code.Ret };
            }
        }

        public override void BackTraceDataFlow(InstructionNode currentInst)
        {
            if (currentInst.InliningProperties.Inlined)
            {
                if (currentInst.InliningProperties.CallNode is ConstructorCallNode)
                {
                    currentInst.DataFlowBackRelated.AddTwoWaySingleIndex(currentInst.InliningProperties.CallNode.DataFlowBackRelated.Where(x => x.ArgIndex == 0).Select(x => x.Argument));
                }
            }
        }
    }
}
