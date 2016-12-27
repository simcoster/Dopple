using System;
using System.Collections.Generic;
using DoppleTry2.InstructionNodes;
using DoppleTry2.VerifierNs;
using System.Linq;

namespace DoppleTry2
{
    internal class ArgIndexVerifier : Verifier
    {
        public ArgIndexVerifier(List<InstructionNode> instructionWrappers) : base(instructionWrappers)
        {
        }

        public override void Verify(InstructionNode instructionWrapper)
        {
            if (instructionWrapper.DataFlowBackRelated.Count == 0)
            {
                return;
            }
            int maxIndex = instructionWrapper.DataFlowBackRelated.Max(x => x.ArgIndex);
            for (int i =0; i <= maxIndex; i++)
            {
                if (!instructionWrapper.DataFlowBackRelated.Any(x => x.ArgIndex == i))
                {
                    throw new Exception("Index missing");
                }
            }
        }
    }
}