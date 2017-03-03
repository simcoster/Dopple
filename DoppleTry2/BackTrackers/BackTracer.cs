using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionModifiers;
using Mono.Cecil.Cil;
using Dopple.InstructionNodes;

namespace Dopple.BackTracers
{
    public abstract class BackTracer
    {
        protected InstructionNode Instruction;
        protected abstract void BackTraceDataFlowSingle(InstructionNode currentInst);
        public void AddBackDataflowConnections(List<InstructionNode> nodes)
        {
            foreach (var node in nodes.Where(x => HandlesCodes.Contains(x.Instruction.OpCode.Code)).OrderByDescending(x => x.InstructionIndex))
            {
                BackTraceDataFlowSingle(node);
            }
        }


        protected virtual bool HasBackDataflowNodes { get; } = true;

        public abstract Code[] HandlesCodes { get; }
    }
}
