using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;
using Mono.Cecil.Cil;

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
            //var index0Arg = instWrapper.DataFlowBackRelated.Where(x => x.ArgIndex == 0).Select(x => x.Argument).ToArray();
            //var index1Arg = instWrapper.DataFlowBackRelated.Where(x => x.ArgIndex == 1).Select(x => x.Argument).ToArray();
            //Func<InstructionNode, bool> predicate = x => CodeGroups.StElemCodes.Contains(x.Instruction.OpCode.Code) &&
            //                                                 x.DataFlowBackRelated.Where(y => y.ArgIndex == 0).Select(y => y.Argument).SequenceEqual(index0Arg) &&
            //                                                 x.DataFlowBackRelated.Where(y => y.ArgIndex == 1).Select(y => y.Argument).SequenceEqual(index1Arg);
            //var found = _SingleIndexBackSearcher.SafeSearchBackwardsForDataflowInstrcutions(x => CodeGroups.StElemCodes.Contains(x.Instruction.OpCode.Code), instWrapper);
            //var foundArgs = found.Select(x => x.DataFlowBackRelated.Select(y => y.ArgIndex + "=" + y.Argument.InstructionIndex).Aggregate((y, z) => (y + " , " + z)));
            //return _SingleIndexBackSearcher.SafeSearchBackwardsForDataflowInstrcutions(predicate, instWrapper);
            throw new Exception();
        }
    }
}
