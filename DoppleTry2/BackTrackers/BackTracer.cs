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
        protected readonly List<InstructionNode> InstructionNodes;
        protected BackTracer(List<InstructionNode> instructionNodes)
        {
            InstructionNodes = instructionNodes;
        }
        protected abstract void InnerAddBackDataflowConnections(InstructionNode currentInst);
        public void AddBackDataflowConnections(InstructionNode currentInst)
        {
            if (currentInst.DoneBackTracers.Contains(GetType()))
            {
                return;
            }
            InnerAddBackDataflowConnections(currentInst);
            currentInst.DoneBackTracers.Add(GetType());
        }


        protected virtual bool HasBackDataflowNodes { get; } = true;

        public abstract Code[] HandlesCodes { get; }
    }
}
