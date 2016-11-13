using System;
using System.Collections.Generic;
using DoppleTry2.InstructionWrappers;


namespace DoppleTry2.Verifier
{
    interface IVerifier
    {
        void Verify(IEnumerable<InstructionWrapper> instructionWrappers);
    }
}