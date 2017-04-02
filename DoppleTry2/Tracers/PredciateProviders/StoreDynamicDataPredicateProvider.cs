using Dopple.InstructionNodes;

namespace Dopple.Tracers.PredciateProviders
{
    abstract class StoreDynamicDataPredicateProvider
    {
        public abstract PredicateAndNode GetMatchingLoadPredicate(InstructionNode storeNode);
        public abstract bool IsRelevant(InstructionNode storeNode);
    }
}
