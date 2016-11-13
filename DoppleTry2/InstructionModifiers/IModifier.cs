﻿using DoppleTry2.InstructionWrappers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.InstructionModifiers
{
    interface IModifier
    {
        void Modify(List<InstructionWrapper> instructionWrappers);
    }

    interface IPreBacktraceModifier : IModifier {}
    interface IPostBackTraceModifier : IModifier { }
}
