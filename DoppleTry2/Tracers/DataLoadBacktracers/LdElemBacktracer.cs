using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;
using Mono.Cecil.Cil;

namespace Dopple.BackTracers
{
    class LdElemBacktracer : DynamicDataBacktracer
    {
        public override Code[] HandlesCodes
        {
            get
            {
                return CodeGroups.LdElemCodes;
            }
        }   

        protected override Predicate<InstructionNode> GetPredicate(InstructionNode instructionNode)
        {
            var index0Arg = instructionNode.DataFlowBackRelated.Where(x => x.ArgIndex == 0).Select(x => x.Argument).ToArray();
            var index1Arg = instructionNode.DataFlowBackRelated.Where(x => x.ArgIndex == 1).Select(x => x.Argument).ToArray();
            return (x => x is StElemInstructionNode &&
                       x.DataFlowBackRelated.Where(y => y.ArgIndex == 0).Select(y => y.Argument).SequenceEqual(index0Arg) &&
                       x.DataFlowBackRelated.Where(y => y.ArgIndex == 1).Select(y => y.Argument).SequenceEqual(index1Arg)
                       );
        }
    }
}
