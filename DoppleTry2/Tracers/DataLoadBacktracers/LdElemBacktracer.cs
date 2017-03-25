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
            var arrayArgs = instructionNode.DataFlowBackRelated.Where(x => x.ArgIndex == 0).SelectMany(x => x.Argument.GetDataOriginNodes()).ToArray();
            var indexArgs = instructionNode.DataFlowBackRelated.Where(x => x.ArgIndex == 1).SelectMany(x => x.Argument.GetDataOriginNodes()).ToArray();
            return (x => x is StElemInstructionNode && ArrayAndIndexMatch((StElemInstructionNode) x, arrayArgs, indexArgs));
        }

        public static bool ArrayAndIndexMatch(StElemInstructionNode stEelemToCheck, IEnumerable<InstructionNode> arrayArgs, IEnumerable<InstructionNode> indexArgs)
        {
            return   stEelemToCheck.DataFlowBackRelated.Where(y => y.ArgIndex == 0).SelectMany(y => y.Argument.GetDataOriginNodes()).SequenceEqual(arrayArgs) &&
                     stEelemToCheck.DataFlowBackRelated.Where(y => y.ArgIndex == 1).SelectMany(y => y.Argument.GetDataOriginNodes()).All(y => HaveEquivilentIndexNode(y, indexArgs));
        }

        public static bool HaveEquivilentIndexNode(InstructionNode indexNodeToMatch, IEnumerable<InstructionNode> indexArgs)
        {
            if (indexNodeToMatch is LdImmediateInstNode)
            {
                int immediateValueToMatch = ((LdImmediateInstNode) indexNodeToMatch).ImmediateIntValue;
                return indexArgs.Any(x => x is LdImmediateInstNode && ((LdImmediateInstNode) x).ImmediateIntValue == immediateValueToMatch);
            }
            else
            {
                return indexArgs.Contains(indexNodeToMatch);
            }
        }
    }
}
