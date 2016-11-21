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
        protected override IEnumerable<IEnumerable<InstructionWrapper>> GetDataflowBackRelated(InstructionWrapper instWrapper)
        {
            var foundInstructions = new List<List<InstructionWrapper>>();
            for (int i = 0; i < instWrapper.StackPopCount; i++)
            {
                var argumentGroup = BackSearcher.SearchBackwardsForDataflowInstrcutions(InstructionWrappers, x => x.StackPushCount > 0, instWrapper);
                foundInstructions.Add(argumentGroup);
                foreach (var arg in argumentGroup)
                {
                    arg.StackPushCount--;
                }
            }
            instWrapper.StackPopCount = 0;
            return foundInstructions;
        }

        public void AddBackDataflowConnectionsInFuncBoundry(InstructionWrapper currentInst)
        {
            var CallWrappersFlowBackup = new Dictionary<InstructionWrapper, List<InstructionWrapper>>();
            BackupCallsProgramFlow(CallWrappersFlowBackup);
            var backRelatedGroups = new List<List<InstructionWrapper>>();
            for (int i = 0; i < currentInst.StackPopCount; i++)
            {
                var argumentGroup = BackSearcher.SafeSearchBackwardsForDataflowInstrcutions(InstructionWrappers, x => x.StackPushCount > 0, currentInst);
                if (argumentGroup.Count == 0)
                {
                    return;
                }
                currentInst.BackDataFlowRelated.AddWithNewIndex(argumentGroup);
                currentInst.StackPopCount--;
                foreach (var backInst in argumentGroup)
                {
                    backInst.ForwardDataFlowRelated.AddWithNewIndex(currentInst);
                    backInst.StackPushCount--;
                }
            }
            RestoreCallsProgramFlow(CallWrappersFlowBackup);
        }

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