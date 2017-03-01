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
    class LdFldBacktracer : SingeIndexBackTracer
    {
        public override Code[] HandlesCodes
        {
            get
            {
                return new[] { Code.Ldfld };
            }
        }

        protected override IEnumerable<InstructionNode> GetDataflowBackRelatedArgGroup(InstructionNode instructionNode)
        {
            var objectInstanceArgs = instructionNode.DataFlowBackRelated.Where(x => x.ArgIndex == 0).Select(x => x.Argument).ToArray();
            FieldReference fieldDefinitionArg = (FieldReference)instructionNode.Instruction.Operand;
            Func<InstructionNode, bool> predicate = x => x.Instruction.OpCode.Code == Code.Stfld &&
                                                             x.DataFlowBackRelated.Where(y => y.ArgIndex == 0).Select(y => y.Argument).SequenceEqual(objectInstanceArgs) &&
                                                             ((FieldReference)x.Instruction.Operand).MetadataToken == fieldDefinitionArg.MetadataToken;
            var found = SingleIndexBackSearcher.SafeSearchBackwardsForDataflowInstrcutions(predicate, instructionNode);
            var found2 = SingleIndexBackSearcher.SafeSearchBackwardsForDataflowInstrcutions(x => x.Instruction.OpCode.Code == Code.Stfld && ((FieldReference) x.Instruction.Operand).Name == fieldDefinitionArg.Name, instructionNode);

            if (found.Count ==0)
            {
                Console.WriteLine("notine foound for " + instructionNode.InstructionIndex);
            }
            return SingleIndexBackSearcher.SafeSearchBackwardsForDataflowInstrcutions(predicate, instructionNode);
        }
    }
}
