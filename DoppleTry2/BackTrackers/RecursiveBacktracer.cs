using System.Collections.Generic;
using DoppleTry2.InstructionNodes;

namespace DoppleTry2.BackTrackers
{
    public abstract class RecursiveBacktracer : BackTracer
    {
        protected BackTraceManager backtraceManager;

        public RecursiveBacktracer(List<InstructionNode> instructionNodes, BackTraceManager backtraceManager) : base(instructionNodes)
        {
            this.backtraceManager = backtraceManager;
        }
    }
}