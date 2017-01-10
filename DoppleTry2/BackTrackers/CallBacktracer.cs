using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionNodes;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace DoppleTry2.BackTrackers
{
    class CallBacktracer : RecursiveBacktracer
    {
        private readonly StackPopBackTracer stackPopBackTracer;

        public CallBacktracer(List<InstructionNode> instructionNodes, BackTraceManager backtraceManager) : base(instructionNodes, backtraceManager)
        {
            stackPopBackTracer = new StackPopBackTracer(instructionNodes, backtraceManager);
        }

        public override Code[] HandlesCodes
        {
            get
            {
                return CodeGroups.CallCodes;
            }
        }

        protected override void InnerAddBackDataflowConnections(InstructionNode currentInst)
        {
            CallNode callNode = (CallNode) currentInst;
            stackPopBackTracer.AddBackDataflowConnections(currentInst);
            if (callNode.TargetMethod.HasThis)
            {
                currentInst.DataFlowBackRelated.ForEach(x => x.ArgIndex++);
            }
        }
    }
}
