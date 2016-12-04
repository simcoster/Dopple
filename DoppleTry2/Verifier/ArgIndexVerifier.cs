using System;
using System.Collections.Generic;
using DoppleTry2.InstructionWrappers;
using DoppleTry2.VerifierNs;
using System.Linq;

namespace DoppleTry2
{
    internal class ArgIndexVerifier : Verifier
    {
        public ArgIndexVerifier(List<InstructionWrapper> instructionWrappers) : base(instructionWrappers)
        {
        }

        public override void Verify(InstructionWrapper instructionWrapper)
        {
            if (instructionWrapper.BackDataFlowRelated.Count == 0)
            {
                return;
            }
            int maxIndex = instructionWrapper.BackDataFlowRelated.Max(x => x.ArgIndex);
            for (int i =0; i <= maxIndex; i++)
            {
                if (!instructionWrapper.BackDataFlowRelated.Any(x => x.ArgIndex == i))
                {
                    throw new Exception("Index missing");
                }
            }
        }
    }
}