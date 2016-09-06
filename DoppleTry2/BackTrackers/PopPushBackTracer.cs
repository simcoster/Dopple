using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    public class StackPopBackTracer : BackTracer
    {
        protected override IEnumerable<int> GetDataflowBackRelatedIndices(int instructionIndex, Node currentNode)
        {
            List<int> foundIndexes = new List<int>();
            for (int i = 0; i < InstructionsWrappers[instructionIndex].StackPopCount; i++)
            {
                foundIndexes.AddRange(SearchBackwardsForDataflowInstrcutions(x => x.StackPushCount > 0, instructionIndex));
                foreach (var foundIndex in foundIndexes)
                {
                    InstructionsWrappers[foundIndex].StackPushCount--;
                }
            }
            return  foundIndexes;
        }

        public override Code[] HandlesCodes => typeof(OpCodes).GetFields()
                    .Select(x => x.GetValue(null))
                    .Cast<OpCode>()
                    .Where(x => x.StackBehaviourPop != StackBehaviour.Pop0)
                    .Select(x => x.Code).ToArray();

        public StackPopBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }
    }
}