using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    public class StackPopBackTracer : BackTracer
    {
        protected override IEnumerable<InstructionWrapper> GetDataflowBackRelatedIndices(InstructionWrapper instWrapper)
        {
            var foundInstructions = new List<InstructionWrapper>();
            for (int i = 0; i < instWrapper.StackPopCount; i++)
            {
                foundInstructions.AddRange(SearchBackwardsForDataflowInstrcutions(x => x.StackPushCount > 0, instWrapper));
                foreach (var foundInstruction in foundInstructions)
                {
                    foundInstruction.StackPushCount--;
                }
            }
            return foundInstructions;
        }

        public override Code[] HandlesCodes => typeof(OpCodes).GetFields()
                    .Select(x => x.GetValue(null))
                    .Cast<OpCode>()
                    .Where(x => x.StackBehaviourPop != StackBehaviour.Pop0 && x.Code != Code.Newarr)
                    .Select(x => x.Code).ToArray();

        public StackPopBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }
    }
}