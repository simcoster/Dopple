using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;

namespace Dopple.Tracers.PredciateProviders
{
    class StElemPredicateProvider : StoreDynamicDataPredicateProvider
    {
        public override Predicate<InstructionNode> GetMatchingLoadPredicate(InstructionNode instructionNode)
        {
            var arrayArgs = instructionNode.DataFlowBackRelated.Where(x => x.ArgIndex == 0).SelectMany(x => x.Argument.GetDataOriginNodes()).ToArray();
            var indexArgs = instructionNode.DataFlowBackRelated.Where(x => x.ArgIndex == 1).SelectMany(x => x.Argument.GetDataOriginNodes()).ToArray();
            Predicate<InstructionNode> predicate = (x => x is LdElemInstructionNode && ArrayAndIndexMatch((LdElemInstructionNode) x, arrayArgs, indexArgs));
            return new PredicateAndNode() { Predicate = predicate, StoreNode = instructionNode };
        }

        public override bool IsRelevant(InstructionNode storeNode)
        {
            return storeNode is StElemInstructionNode;
        }

        public static bool ArrayAndIndexMatch(LdElemInstructionNode ldEelemToCheck, IEnumerable<InstructionNode> arrayArgs, IEnumerable<InstructionNode> indexArgs)
        {
            return ldEelemToCheck.DataFlowBackRelated.Where(y => y.ArgIndex == 0).SelectMany(y => y.Argument.GetDataOriginNodes()).SequenceEqual(arrayArgs) &&
                     ldEelemToCheck.DataFlowBackRelated.Where(y => y.ArgIndex == 1).SelectMany(y => y.Argument.GetDataOriginNodes()).All(y => HaveEquivilentIndexNode(y, indexArgs));
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
