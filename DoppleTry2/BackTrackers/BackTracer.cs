using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionModifiers;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    public abstract class BackTracer
    {
        protected InstructionWrapper Instruction;
        protected readonly List<InstructionWrapper> InstructionsWrappers;

        protected readonly IEnumerable<OpCode> AllOpCodes =
            typeof(OpCodes).GetFields().Select(x => x.GetValue(null)).Cast<OpCode>();

        protected BackTracer(List<InstructionWrapper> instructionsWrappers)
        {
            InstructionsWrappers = instructionsWrappers;
        }

        public void AddBackDataflowConnections(InstructionWrapper currentInst)
        {
            currentInst.WasTreated = true;
            if (!HasBackDataflowNodes)
            {
                currentInst.HasBackRelated = false;
                return;
            }
            var backRelatedInsts = GetDataflowBackRelatedIndices(currentInst);
            foreach (var backRelatedInst in backRelatedInsts)
            {
                currentInst.BackDataFlowRelated.Add(backRelatedInst);
            }
        }

        protected virtual bool HasBackDataflowNodes { get; } = true;

        protected abstract IEnumerable<InstructionWrapper> GetDataflowBackRelatedIndices(InstructionWrapper instWrapper);

        public abstract Code[] HandlesCodes { get; }

        protected List<InstructionWrapper> SearchBackwardsForDataflowInstrcutions(Func<InstructionWrapper, bool> predicate,
            InstructionWrapper startInstruction)
        {
            List<InstructionWrapper> indexes = SafeSearchBackwardsForDataflowInstrcutions(predicate, startInstruction);
            if (indexes.Count == 0)
            {
                throw new Exception("Reached first instWrapper without correct one found");
            }
            return indexes;
        }

        protected List<InstructionWrapper> SafeSearchBackwardsForDataflowInstrcutions(Func<InstructionWrapper, bool> predicate,
           InstructionWrapper startInstruction)
        {
            List<InstructionWrapper> foundInstructions = new List<InstructionWrapper>();
            int index = InstructionsWrappers.IndexOf(startInstruction)- 1;
            if (index <0)
                return new List<InstructionWrapper>();
            bool done = false;
            while (done == false)
            {
                var currInstruction = InstructionsWrappers[index];
                if (predicate.Invoke(currInstruction))
                {
                    foundInstructions.Add(currInstruction);
                    done = true;
                }
                else if (InlineCall.CallOpCodes.Contains(currInstruction.Instruction.OpCode.Code)||
                    currInstruction.Instruction.OpCode.Code == Code.Ret ||
                    currInstruction.BackProgramFlow.Count ==0)
                {
                    done = true;
                }
                else if(currInstruction.BackProgramFlow.Count == 1)
                {
                    index = InstructionsWrappers.IndexOf(currInstruction.BackProgramFlow[0]);
                }
                else
                {
                    foreach (var instructionWrapper in currInstruction.BackProgramFlow)
                    {
                        IEnumerable<InstructionWrapper> branchindexes = 
                            SafeSearchBackwardsForDataflowInstrcutions(predicate, instructionWrapper);
                        foundInstructions.AddRange(branchindexes);
                    }
                    done = true;
                }
            }
            return foundInstructions;
        }

        protected IEnumerable<InstructionWrapper> GetStackPushAncestor(InstructionWrapper currInst)
        {
            var instWrapper = currInst;
            while (true)
            {
                switch (currInst.BackDataFlowRelated.Count)
                {
                    case 0:
                        return new [] { instWrapper } ;
                        break;
                    case 1:
                        instWrapper = instWrapper.BackDataFlowRelated[0];
                        break;
                    default:
                        return instWrapper.BackDataFlowRelated.SelectMany(GetStackPushAncestor);
                }
            }
        }

        protected bool HaveCommonStackPushAncestor(InstructionWrapper firstInstruction, InstructionWrapper secondInstructions)
        {
            var firstAncestors = GetStackPushAncestor(firstInstruction).ToArray();
            var secondAncestors = GetStackPushAncestor(secondInstructions).ToArray();

            foreach (var firstAncestor in firstAncestors)
            {
                foreach (var secondAncestor in secondAncestors)
                {
                    if (SameOrEquivilent(firstAncestor, secondAncestor))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected bool SameOrEquivilent(InstructionWrapper firstAncestor, InstructionWrapper secondAncestor)
        {
            return firstAncestor == secondAncestor ||
                   (firstAncestor.Instruction.OpCode.Code == secondAncestor.Instruction.OpCode.Code &&
                    firstAncestor.Instruction.Operand == secondAncestor.Instruction.Operand);
        }

    }
}
