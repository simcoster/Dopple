using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Dopple.Tracers.PredciateProviders
{
    class StFldPredicateProvider : StoreDynamicDataPredicateProvider
    {
        public override PredicateAndNode GetMatchingLoadPredicate(InstructionNode storeNode)
        {
            var objectInstanceArgs = storeNode.DataFlowBackRelated.Where(x => x.ArgIndex == 0).SelectMany(x => x.Argument.GetDataOriginNodes()).ToArray();
            FieldReference fieldDefinitionArg = (FieldReference) storeNode.Instruction.Operand;
            Predicate<InstructionNode> predicate = x => x is LoadFieldNode &&
                                                        x.DataFlowBackRelated.Where(y => y.ArgIndex == 0).SelectMany(y => y.Argument.GetDataOriginNodes()).Intersect(objectInstanceArgs).Count() >0 &&
                                                        ((FieldReference) x.Instruction.Operand).MetadataToken == fieldDefinitionArg.MetadataToken;
            return new PredicateAndNode() { Predicate = predicate, StoreNode = storeNode };

        }

        public override bool IsRelevant(InstructionNode storeNode)
        {
            return storeNode is StoreFieldNode;
        }
    }
}
