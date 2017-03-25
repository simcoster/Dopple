using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace Dopple.BackTracers
{
    class LdFldBacktracer : DynamicDataBacktracer
    {
        public override Code[] HandlesCodes
        {
            get
            {
                return new[] { Code.Ldfld };
            }
        }
      
        protected override Predicate<InstructionNode> GetPredicate(InstructionNode instructionNode)
        {
            var objectInstanceArgs = instructionNode.DataFlowBackRelated.Where(x => x.ArgIndex == 0).SelectMany(x => x.Argument.GetDataOriginNodes()).ToArray();
            FieldReference fieldDefinitionArg = (FieldReference) instructionNode.Instruction.Operand;
            Predicate<InstructionNode> predicate = x => x.Instruction.OpCode.Code == Code.Stfld &&
                                                        x.DataFlowBackRelated.Where(y => y.ArgIndex == 0).SelectMany(y => y.Argument.GetDataOriginNodes()).SequenceEqual(objectInstanceArgs) &&
                                                        ((FieldReference) x.Instruction.Operand).MetadataToken == fieldDefinitionArg.MetadataToken;
            return predicate;
        }
    }
}
