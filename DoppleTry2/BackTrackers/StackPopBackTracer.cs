using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    public class StackPopBackTracer : BackTracer
    {
        protected override IEnumerable<IEnumerable<InstructionWrapper>> GetDataflowBackRelated(InstructionWrapper instWrapper)
        {
            var foundInstructions = new List<List<InstructionWrapper>>();
            for (int i = 0; i < instWrapper.StackPopCount; i++)
            {
                //var argumentGroup = SearchBackwardsForDataflowInstrcutions(x => x.StackPushCount > 0, instWrapper);
                var argumentGroup = BackSearcher.SafeSearchBackwardsForDataflowInstrcutions(InstructionWrappers, x => x.StackPushCount > 0, instWrapper);
                if (argumentGroup.Count ==0)
                {
                    instWrapper.MarkForDebugging = true;
                }
                foundInstructions.Add(argumentGroup);
                foreach (var arg in argumentGroup)
                {
                    arg.StackPushCount--;
                }
            }
            instWrapper.StackPopCount = 0;
            return foundInstructions;
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