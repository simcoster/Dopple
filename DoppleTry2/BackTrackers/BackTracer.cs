using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionModifiers;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    public abstract class BackTracer
    {
        protected InstructionWrapper Instruction;
        protected readonly List<InstructionWrapper> InstructionWrappers;

        protected readonly IEnumerable<OpCode> AllOpCodes =
            typeof(OpCodes).GetFields().Select(x => x.GetValue(null)).Cast<OpCode>();

        protected BackTracer(List<InstructionWrapper> instructionsWrappers)
        {
            InstructionWrappers = instructionsWrappers;
        }

        public void AddBackDataflowConnections(InstructionWrapper currentInst)
        {
            currentInst.WasTreated = true;
            var backRelatedInsts = GetDataflowBackRelated(currentInst);
            foreach (var backRelatedGroup in backRelatedInsts)
            {
                currentInst.BackDataFlowRelated.AddSingleIndex(backRelatedGroup);
                foreach(var backInst in backRelatedGroup)
                {
                    backInst.ForwardDataFlowRelated.AddSingleIndex(currentInst);
                }
            }
        }

        protected virtual bool HasBackDataflowNodes { get; } = true;

        protected abstract IEnumerable<IEnumerable<InstructionWrapper>> GetDataflowBackRelated(InstructionWrapper instWrapper);

        public abstract Code[] HandlesCodes { get; }

        /*
        protected List<InstructionWrapper> SearchBackwardsForDataflowInstrcutions(Func<InstructionWrapper, bool> predicate,
            InstructionWrapper startInstruction)
        {
            return BackSearcher.SearchBackwardsForDataflowInstrcutions(InstructionWrappers , predicate, startInstruction);
        }

        protected List<InstructionWrapper> SafeSearchBackwardsForDataflowInstrcutions(Func<InstructionWrapper, bool> predicate,
           InstructionWrapper startInstruction)
        {
            return BackSearcher.SafeSearchBackwardsForDataflowInstrcutions(InstructionWrappers,predicate,startInstruction);
        }

        protected List<InstructionWrapper> SafeSearchBackwardsForDataflowInstrcutionsRec(Func<InstructionWrapper, bool> predicate,
        InstructionWrapper startInstruction, List<InstructionWrapper> visitedInstructions)
        {
            return BackSearcher.SafeSearchBackwardsForDataflowInstrcutionsRec(InstructionWrappers, predicate,startInstruction, new List<InstructionWrapper>());
        }

        protected IEnumerable<InstructionWrapper> GetStackPushAncestor(InstructionWrapper startInst, List<InstructionWrapper> visited = null)
        {
            return BackSearcher.GetStackPushAncestor(startInst, visited);
        }

        protected bool HaveCommonStackPushAncestor(InstructionWrapper firstInstruction, InstructionWrapper secondInstructions)
        {
            return BackSearcher.HaveCommonStackPushAncestor(firstInstruction, secondInstructions);
        }

        protected bool SameOrEquivilent(InstructionWrapper firstAncestor, InstructionWrapper secondAncestor)
        {
            return BackSearcher.SameOrEquivilent(firstAncestor, secondAncestor);

        }

    */

    }

   
}
