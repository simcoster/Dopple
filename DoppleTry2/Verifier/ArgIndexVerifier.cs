using System;
using System.Collections.Generic;
using DoppleTry2.InstructionNodes;
using DoppleTry2.VerifierNs;
using System.Linq;

namespace DoppleTry2
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