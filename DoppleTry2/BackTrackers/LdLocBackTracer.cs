using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    public class LdLocBackTracer : BackTracer
    {
        private readonly Dictionary<Code, Code> _ldStLocs = new Dictionary<Code, Code>()
        {
            {Code.Ldloc_0, Code.Stloc_0},
            {Code.Ldloc_1, Code.Stloc_1},
            {Code.Ldloc_2, Code.Stloc_2},
            {Code.Ldloc_3, Code.Stloc_3},
            {Code.Ldloc, Code.Stloc},
        };


        protected override int[] GetBackRelatedIndices(int instructionIndex, Node currentNode)
        {
            var code = InstructionsWrappers[instructionIndex].Instruction.OpCode.Code;
            var index = SearchBackwardsForInstrcution((x) => x.Instruction.OpCode.Code == _ldStLocs[code], instructionIndex);
            return new[] { index };
        }

        public override Code[] HandlesCodes => new []{Code.Ldloc_0, Code.Ldloc_1, Code.Ldloc_2, Code.Ldloc, Code.Ldloc_S};

        public LdLocBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }
    }
}