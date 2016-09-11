using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    class ArrayLdBackTracer : BackTracer
    {
        public ArrayLdBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override IEnumerable<InstructionWrapper> GetDataflowBackRelatedIndices(InstructionWrapper instWrapper)
        {
            throw new NotImplementedException();
        }

        public override Code[] HandlesCodes => new[] {Code.Ldlen};
    }
}
