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

        protected override void BackTraceDataFlowSingle(InstructionNode currentInst)
        {
            if (currentInst.InliningProperties.Inlined)
            {
                if (currentInst.InliningProperties.CallNode is ConstructorCallNode)
                {
                    currentInst.DataFlowBackRelated.AddTwoWaySingleIndex(currentInst.DataFlowBackRelated.Where(x => x.ArgIndex == 0).Select(x => x.Argument));
                }
                currentInst.DataFlowForwardRelated.AddTwoWay(currentInst.InliningProperties.CallNode.DataFlowForwardRelated);
            }
        }
    }
}
