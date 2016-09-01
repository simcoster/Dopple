using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    class TrivialBacktracer : BackTracer
    {
        public TrivialBacktracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override int[] GetBackRelatedIndices(int instructionIndex, Node currentNode)
        {
            return new[] {instructionIndex-1};
        }

        public override Code[] HandlesCodes => new []
        {
            Code.Jmp,
        }
    }
}
