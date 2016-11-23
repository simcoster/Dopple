using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionModifiers;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionWrappers;

namespace DoppleTry2.BackTrackers
{
    public abstract class BackTracer
    {
        protected InstructionWrapper Instruction;
        protected readonly List<InstructionWrapper> InstructionWrappers;

        protected readonly IEnumerable<OpCode> AllOpCodes =
            typeof(OpCodes).GetFields().Select(x => x.GetValue(null)).Cast<OpCode>();

        protected BackTracer(List<InstructionWrapper> instructionsWrappers)
        {
            InstructionWrappers = instructionsWrappers;
        }
        public abstract void AddBackDataflowConnections(InstructionWrapper currentInst);
       
        protected virtual bool HasBackDataflowNodes { get; } = true;

        public abstract Code[] HandlesCodes { get; }
    }
}
