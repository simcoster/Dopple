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

        public void AddBackDataflowConnections(InstructionWrapper currentInst)
        {
            if (currentInst.DoneBackTracers.Contains(GetType()))
            {
                return;
            }
            var backRelatedInsts = GetDataflowBackRelated(currentInst);

            foreach (var backRelatedGroup in backRelatedInsts)
            {
                currentInst.BackDataFlowRelated.AddWithNewIndex(backRelatedGroup);
                foreach(var backInst in backRelatedGroup)
                {
                    backInst.ForwardDataFlowRelated.AddWithNewIndex(currentInst);
                }
            }
            currentInst.DoneBackTracers.Add(GetType());
        }

        protected virtual bool HasBackDataflowNodes { get; } = true;

        protected abstract IEnumerable<IEnumerable<InstructionWrapper>> GetDataflowBackRelated(InstructionWrapper instWrapper);

        public abstract Code[] HandlesCodes { get; }
    }
}
