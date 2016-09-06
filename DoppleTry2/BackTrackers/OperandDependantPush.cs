using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    class OperandDependantPush : BackTracer
    {
        //Need to check if 2 contain the same operand, if they do, they will supply the same value
        public OperandDependantPush(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override IEnumerable<int>[] GetDataflowBackRelatedIndices(int instructionIndex, Node currentNode)
        {
            throw new NotImplementedException();
        }

        public override Code[] HandlesCodes => new[] {Code.Ldfld, Code.Ldflda, Code.Ldtoken, Code.Sizeof };
    }
}
