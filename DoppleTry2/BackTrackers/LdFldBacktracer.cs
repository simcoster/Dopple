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
        public LdFldBacktracer(List<InstructionNode> instructionNodes) : base(instructionNodes)
        {
        }

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
            FieldDefinition fieldDefinitionArg = (FieldDefinition)instructionNode.Instruction.Operand;
            Func<InstructionNode, bool> predicate = x => x.Instruction.OpCode.Code == Code.Stfld &&
                                                             x.DataFlowBackRelated.Where(y => y.ArgIndex == 0).Select(y => y.Argument).SequenceEqual(objectInstanceArgs) &&
                                                             x.Instruction.Operand == fieldDefinitionArg;
            //var found = _SingleIndexBackSearcher.SafeSearchBackwardsForDataflowInstrcutions(x => CodeGroups.StElemCodes.Contains(x.Instruction.OpCode.Code), instructionNode);
            //var foundArgs = found.Select(x => x.DataFlowBackRelated.Select(y => y.ArgIndex + "=" + y.Argument.InstructionIndex).Aggregate((y, z) => (y + " , " + z)));
            return _SingleIndexBackSearcher.SafeSearchBackwardsForDataflowInstrcutions(predicate, instructionNode);
        }
    }
}
