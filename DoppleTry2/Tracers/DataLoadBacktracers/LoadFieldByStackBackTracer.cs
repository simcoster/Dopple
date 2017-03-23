using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using Dopple.InstructionNodes;

namespace Dopple.BackTracers
{
    class LoadFieldByStackBackTracer : DynamicDataBacktracer
    {
        protected override Predicate<InstructionNode> GetPredicate(InstructionNode instructionNode)
        {
            var objectArgs = instructionNode.DataFlowBackRelated.Where(x => x.ArgIndex == 0).Select(x => x.Argument).ToArray();
            Predicate<InstructionNode> predicate = x =>
                                  x is StoreFieldNode &&
                                  x.Instruction.Operand == instructionNode.Instruction.Operand &&
                                  x.DataFlowBackRelated.Where(y => y.ArgIndex == 0).Select(y => y.Argument).SequenceEqual(objectArgs);
            return predicate;
        }

        public override Code[] HandlesCodes => new[] {Code.Ldfld, Code.Ldflda};

    }
}
