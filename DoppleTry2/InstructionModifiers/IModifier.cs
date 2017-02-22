using Dopple.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.InstructionModifiers
{
    interface IModifier
    {
        void Modify(List<InstructionNode> instructionWrappers);
    }

    interface IPreBacktraceModifier : IModifier {}
    interface IPostBackTraceModifier : IModifier { }
}
