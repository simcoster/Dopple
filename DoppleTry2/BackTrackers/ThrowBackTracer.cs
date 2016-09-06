using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    class ThrowBackTracer : BackTracer
    {
        public ThrowBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override IEnumerable<int> GetDataflowBackRelatedIndices(int instructionIndex, Node currentNode)
        {
            throw new NotImplementedException();
        }

        public override Code[] HandlesCodes { get; }
    }
}
