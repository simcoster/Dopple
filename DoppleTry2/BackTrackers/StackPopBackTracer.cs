using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionWrappers;

namespace DoppleTry2.BackTrackers
{
    public class StackPopBackTracer : BackTracer
    {
        /// <summary>
        /// This function uses both recursion And mutual recursion with AddBackDataflowConnections
        /// If the search comes accross stackpop instruction wrappers that have not been resolved yet, they must be so
        /// only then can it resolve its
        /// The reason is the stack structure - push will be pulled by its nearest neighbour
        /// Since its difficult to know ahead who will those couples be, we just start going and then deal with
        /// those when they come by.
        /// </summary>
        /// <param name="instructionWrappers"></param>
        /// <param name="predicate"></param>
        /// <param name="startInstruction"></param>
        /// <param name="visitedInstructions"></param>
        /// <returns></returns>
        private IEnumerable<InstructionWrapper> SearchAndAddDataflowInstrcutions(List<InstructionWrapper> instructionWrappers, Func<InstructionWrapper, bool> predicate,
        InstructionWrapper startInstruction, List<InstructionWrapper> visitedInstructions = null)
        {
            var foundInstructions = new List<InstructionWrapper>();
            if (visitedInstructions == null)
            {
                visitedInstructions = new List<InstructionWrapper>();
            }
            foreach (var backInstructionWrapper in startInstruction.BackProgramFlow)
            {
                if (visitedInstructions.Contains(backInstructionWrapper))
                {
                    continue;
                }
                else
                {
                    visitedInstructions.Add(backInstructionWrapper);
                }
                bool mustBacktraceCurrentFirst = backInstructionWrapper.StackPopCount != 0;
                if (mustBacktraceCurrentFirst)
                {
                    AddBackDataflowConnections(backInstructionWrapper);
                }
                if (predicate.Invoke(backInstructionWrapper))
                {
                    foundInstructions.Add(backInstructionWrapper);
                }
                else
                {
                    foundInstructions.AddRange(SearchAndAddDataflowInstrcutions(instructionWrappers,predicate, backInstructionWrapper, visitedInstructions));
                }
            }
            return foundInstructions;
        }

        public override void AddBackDataflowConnections(InstructionWrapper currentInst)
        {
            for (int i = 0; i < currentInst.StackPopCount; i++)
            {
                var argumentGroup = SearchAndAddDataflowInstrcutions(InstructionWrappers, x => x.StackPushCount > 0, currentInst);
                if (argumentGroup.Count() ==0
                    )
                {
                    throw new Exception("Couldn't find back data connections");
                }
                currentInst.BackDataFlowRelated.AddWithNewIndex(argumentGroup);
                foreach (InstructionWrapper arg in argumentGroup)
                {
                    arg.ForwardDataFlowRelated.AddWithNewIndex(currentInst);
                    arg.StackPushCount--;
                }
            }
            currentInst.StackPopCount = 0;
        }

        //public void TryAddBackDataflowConnectionsInFuncBoundry(InstructionWrapper currentInst)
        //{
        //    var CallWrappersFlowBackup = new Dictionary<InstructionWrapper, List<InstructionWrapper>>();
        //    BackupCallsProgramFlow(CallWrappersFlowBackup);
        //    var backRelatedGroups = new List<List<InstructionWrapper>>();
        //    for (int i = currentInst.StackPopCount; i > 0; i--)
        //    {
        //        var argumentGroup = SafeSearchBackwardsForDataflowInstrcutions(InstructionWrappers, x => x.StackPushCount > 0, currentInst);
        //        if (argumentGroup.Count == 0)
        //        {
        //            return;
        //        }
        //        currentInst.BackDataFlowRelated.AddWithNewIndex(argumentGroup);
        //        currentInst.StackPopCount--;
        //        foreach (var backInst in argumentGroup)
        //        {
        //            backInst.ForwardDataFlowRelated.AddWithNewIndex(currentInst);
        //            backInst.StackPushCount--;
        //        }
        //    }
        //    RestoreCallsProgramFlow(CallWrappersFlowBackup);
        //}

        private void BackupCallsProgramFlow(Dictionary<InstructionWrapper, List<InstructionWrapper>> CallWrappersFlowBackup)
        {
            foreach (var callInstWrapper in InstructionWrappers.Where(x => CodeGroups.CallCodes.Contains(x.Instruction.OpCode.Code)))
            {
                CallWrappersFlowBackup.Add(callInstWrapper, new List<InstructionWrapper>(callInstWrapper.BackProgramFlow));
                callInstWrapper.BackProgramFlow.Clear();
            }
        }

        private void RestoreCallsProgramFlow(Dictionary<InstructionWrapper, List<InstructionWrapper>> CallWrappersFlowBackup)
        {
            foreach (var callInstWrapper in InstructionWrappers.Where(x => CodeGroups.CallCodes.Contains(x.Instruction.OpCode.Code)))
            {
                callInstWrapper.BackProgramFlow.AddRange(CallWrappersFlowBackup[callInstWrapper]);
            }
        }


        public override Code[] HandlesCodes => typeof(OpCodes).GetFields()
                    .Select(x => x.GetValue(null))
                    .Cast<OpCode>()
                    .Where(x => x.StackBehaviourPop != StackBehaviour.Pop0)
                    .Select(x => x.Code).ToArray();

        public StackPopBackTracer(List<InstructionWrapper> instructionWrappers) : base(instructionWrappers)
        {
        }
    }
}