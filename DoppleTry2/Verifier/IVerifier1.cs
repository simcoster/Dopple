using System;
using System.Collections.Generic;

namespace DoppleTry2.Varifier
{
    interface IVerifier
    {
        void Verify(IEnumerable<InstructionWrapper> instructionWrappers);
    }
}