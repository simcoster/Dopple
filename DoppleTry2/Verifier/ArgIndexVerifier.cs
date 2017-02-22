using System;
using System.Collections.Generic;
using Dopple.InstructionNodes;
using Dopple.VerifierNs;
using System.Linq;

namespace Dopple
{
    internal class ArgIndexVerifier : Verifier
    {
        public ArgIndexVerifier(List<InstructionNode> instructionNodes) : base(instructionNodes)
        {
        }

        public override void Verify(InstructionNode instructionWrapper)
        {
            instructionWrapper.DataFlowBackRelated.CheckNumberings();
        }
    }
}