using System.Collections.Generic;
using Dopple.InstructionNodes;

namespace Dopple.BackTracers
{
    public abstract class RecursiveBacktracer : BackTracer
    {
        protected TraceManager backtraceManager;

        public RecursiveBacktracer(TraceManager backtraceManager)
        {
            this.backtraceManager = backtraceManager;
        }
    }
}