using DoppleTry2.InstructionWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.InstructionWrapperMembers
{
    public class BackFlowList : List<InstructionWrapper>
    {
        public BackFlowList(InstructionWrapper containingWrapper)
        {
            _ContainingWrapper = containingWrapper;
        }
        InstructionWrapper _ContainingWrapper;
        public void RemoveTwoWay (InstructionWrapper backArgToRemove)
        {
            base.Remove(backArgToRemove);
            var forwardArg = backArgToRemove.ForwardProgramFlow.First(x => x == _ContainingWrapper);
            backArgToRemove.ForwardProgramFlow.Remove(forwardArg);
        }
        public void RemoveAllTwoWay (Predicate<InstructionWrapper> predicate)
        {
            foreach (var toRemove in this.Where(x => predicate(x)).ToList())
            {
                RemoveTwoWay(toRemove);
            }
        }
        public void AddTwoWay(InstructionWrapper toAdd)
        {
            base.Add(toAdd);
            toAdd.ForwardProgramFlow.Add(_ContainingWrapper);
        }
        public void AddRangeTwoWay(IEnumerable<InstructionWrapper> rangeToAdd)
        {
            foreach (var backArgToAdd in rangeToAdd)
            {
                AddTwoWay(backArgToAdd);
            }
        }

        [Obsolete ("Please use AddTwoWay instead")]
        public new void Add(InstructionWrapper instructionWrapper)
        {
            base.Add(instructionWrapper);
        }


        [Obsolete("Please use AddRangeTwoWay instead")]
        public new void AddRange(IEnumerable<InstructionWrapper> instructionWrappers)
        {
            base.AddRange(instructionWrappers);
        }

        [Obsolete("Please use RemoveTwoWay instead")]
        public new void Remove(InstructionWrapper instructionWrapper)
        {
            base.Remove(instructionWrapper);
        }

        [Obsolete("Please use RemoveAllTwoWay instead")]
        public new void RemoveAll(Predicate<InstructionWrapper> predicate)
        {
            base.RemoveAll(predicate);
        }

    }
}
