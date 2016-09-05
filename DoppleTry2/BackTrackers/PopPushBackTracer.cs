using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    public class SimplePopPush : BackTracer
    {
        protected override int[] GetDataflowBackRelatedIndices(int instructionIndex, Node currentNode)
        {
            var stackPushingOpCode = new Func<InstructionWrapper, bool>(data => data.StackPushCount > 0);
            int firstInst = SearchBackwardsForInstrcution(stackPushingOpCode, instructionIndex);
            InstructionsWrappers[firstInst].StackPushCount = InstructionsWrappers[firstInst].StackPushCount--;
            return new[] { firstInst };
        }

        public override Code[] HandlesCodes => new []
        {
            Code.Starg, Code.Starg_S,
            Code.Stloc, Code.Stloc_0, Code.Stloc_1, Code.Stloc_2, Code.Stloc_3,Code.Stloc_S, 
            Code.Dup, Code.Pop, 
        };

        public SimplePopPush(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
            var HandlesCodeeees = typeof(OpCodes).GetFields().Select(x => x.GetValue(null)).Cast<OpCode>()
                .Where(x => x.StackBehaviourPop == StackBehaviour.Pop1 && x.StackBehaviourPush == StackBehaviour.Push1);
            var HandlesCodeses = typeof(OpCodes).GetFields().Select(x => x.GetValue(null)).Cast<OpCode>()
                .Where(x => x.StackBehaviourPop == StackBehaviour.Pop0 && x.StackBehaviourPush == StackBehaviour.Pushi);

            HandlesCodes = typeof(OpCodes).GetFields().Select(x => x.GetValue(null)).Cast<OpCode>()
                    .Where(x => x.StackBehaviourPop == StackBehaviour.Pop1 && x.StackBehaviourPush == StackBehaviour.Push1)
                    .Select(x => x.Code).ToArray();
        }
    }
}