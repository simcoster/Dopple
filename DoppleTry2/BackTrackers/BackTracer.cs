using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionModifiers;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionNodes;

namespace DoppleTry2.BackTrackers
{
    public abstract class BackTracer
    {
        protected InstructionNode Instruction;
        protected readonly List<InstructionNode> InstructionWrappers;

        protected readonly IEnumerable<OpCode> AllOpCodes =
            typeof(OpCodes).GetFields().Select(x => x.GetValue(null)).Cast<OpCode>();

        protected BackTracer(List<InstructionNode> instructionsWrappers)
        {
            InstructionWrappers = instructionsWrappers;
        }
        public abstract void AddBackDataflowConnections(InstructionNode currentInst);
       
        protected virtual bool HasBackDataflowNodes { get; } = true;

        public abstract Code[] HandlesCodes { get; }
    }
}
