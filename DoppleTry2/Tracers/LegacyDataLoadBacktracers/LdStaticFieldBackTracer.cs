using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using Dopple.InstructionNodes;
using Mono.Cecil;

namespace Dopple.BackTracers
{
    class LdStaticFieldBackTracer : DynamicDataBacktracer
    {
        protected override Predicate<InstructionNode> GetPredicate(InstructionNode instructionNode)
        {
            Predicate<InstructionNode> predicate = (x => x.Instruction.OpCode.Code == Code.Stsfld);
                                                   //   && ((FieldReference) x.Instruction.Operand).MetadataToken == ((FieldReference) instructionNode.Instruction.Operand).MetadataToken);
            return predicate;
        }

        public override Code[] HandlesCodes => new[] {Code.Ldsfld,Code.Ldsflda};
    }
}
