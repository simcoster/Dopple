using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionNodes;

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
        /// <param name="visitedNodes"></param>
        /// <returns></returns>
        public IEnumerable<InstructionNode> SearchAndAddDataflowInstrcutions(InstructionNode startInstruction, List<InstructionNode> visitedNodes = null)
        {
            var foundInstructions = new List<InstructionNode>();
            if (visitedNodes == null)
            {
                visitedNodes = new List<InstructionNode>();
            }
            foreach (var backNode in startInstruction.ProgramFlowBackRoutes)
            {
                if (visitedNodes.Contains(backNode))
                {
                    continue;
                }
                else
                {
                    visitedNodes.Add(backNode);
                }
                bool mustBacktraceCurrentFirst = backNode.StackPopCount != 0;
                if (mustBacktraceCurrentFirst)
                {
                    AddBackDataflowConnections(backNode);
                }
                if (backNode.StackPushCount >0 )
                {
                    foundInstructions.Add(backNode);
                }
                else
                {
                    foundInstructions.AddRange(SearchAndAddDataflowInstrcutions(backNode, visitedNodes));
                }
            }
            return foundInstructions;
        }

        public override void AddBackDataflowConnections(InstructionNode currentInst)
        {
            for (int i = 0; i < currentInst.StackPopCount; i++)
            {
                var argumentGroup = SearchAndAddDataflowInstrcutions(currentInst);
                if (argumentGroup.Count() ==0
                    )
                {
                    throw new Exception("Couldn't find back data connections");
                }
                currentInst.DataFlowBackRelated.AddWithNewIndex(argumentGroup);
                foreach (InstructionNode arg in argumentGroup)
                {
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

        private void BackupCallsProgramFlow(Dictionary<InstructionNode, List<InstructionNode>> CallWrappersFlowBackup)
        {
            foreach (var callInstWrapper in InstructionWrappers.Where(x => CodeGroups.CallCodes.Contains(x.Instruction.OpCode.Code)))
            {
                CallWrappersFlowBackup.Add(callInstWrapper, new List<InstructionNode>(callInstWrapper.ProgramFlowBackRoutes));
                callInstWrapper.ProgramFlowBackRoutes.Clear();
            }
        }

        private void RestoreCallsProgramFlow(Dictionary<InstructionNode, List<InstructionNode>> CallWrappersFlowBackup)
        {
            foreach (var callInstWrapper in InstructionWrappers.Where(x => CodeGroups.CallCodes.Contains(x.Instruction.OpCode.Code)))
            {
                callInstWrapper.ProgramFlowBackRoutes.AddRangeTwoWay(CallWrappersFlowBackup[callInstWrapper]);
            }
        }


        public override Code[] HandlesCodes => typeof(OpCodes).GetFields()
                    .Select(x => x.GetValue(null))
                    .Cast<OpCode>()
                    .Where(x => x.StackBehaviourPop != StackBehaviour.Pop0)
                    .Select(x => x.Code).ToArray();

        public StackPopBackTracer(List<InstructionNode> instructionWrappers) : base(instructionWrappers)
        {
        }
    }
}