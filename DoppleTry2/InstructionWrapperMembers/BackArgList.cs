using DoppleTry2.InstructionWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2
{
    public class BackArgList : List<IndexedArgument>
    {
        public BackArgList(InstructionWrapper instructionWrapper)
        {
            containingWrapper = instructionWrapper;
        }
        InstructionWrapper containingWrapper;
        public int MaxArgIndex = -1;

        public new void Remove(IndexedArgument backArgToRemove)
        {
            base.Remove(backArgToRemove);
            var forwardArg = backArgToRemove.Argument.ForwardDataFlowRelated.First(x => x == containingWrapper);
            backArgToRemove.Argument.ForwardDataFlowRelated.Remove(forwardArg);
        }
        public new void RemoveAll(Predicate<IndexedArgument> predicate)
        {
            foreach (var toRemove in this.Where(x => predicate(x)))
            {
                Remove(toRemove);
            }
        }
        public new void Add(IndexedArgument toAdd)
        {
            base.Add(toAdd);
            toAdd.Argument.ForwardDataFlowRelated.Add(containingWrapper);
        }
        public new void AddRange(IEnumerable<IndexedArgument> rangeToAdd)
        {
            foreach (var backArgToAdd in rangeToAdd)
            {
                Add(backArgToAdd);
            }
        }

        public void AddWithNewIndex(IEnumerable<InstructionWrapper> backInstructions)
        {
            int index = GetNewIndex();
            AddRange(backInstructions.Select(x => new IndexedArgument(index, x)));
            CheckNumberings();
        }
        public void AddWithNewIndexes(IEnumerable<InstructionWrapper> instructionWrappers)
        {
            foreach (var instWrapper in instructionWrappers)
            {
                AddWithNewIndex(instWrapper);
            }
        }
        public void AddWithExistingIndex(InstructionWrapper backInstruction , int index)
        {
            if (this.Any(x => x.ArgIndex == index && x.Argument == backInstruction))
            {
                return;
            }
            this.Add(new IndexedArgument(index, backInstruction));
            CheckNumberings();
        }
        public void AddWithExistingIndex(IndexedArgument indexedArg)
        {
            AddWithExistingIndex(indexedArg.Argument,indexedArg.ArgIndex);
        }
        public void AddMultipleWithExistingIndex(IEnumerable<InstructionWrapper> instructionWrappers, int index)
        {
            foreach(var instWrapper in instructionWrappers)
            {
                AddWithExistingIndex(instWrapper, index);
            }
        }       

        public void AddPreserveIndexes(BackArgList argList)
        {
            this.AddRange(argList);
            CheckNumberings();
        }
        
        public void AddWithNewIndex(InstructionWrapper backInst)
        {
            AddWithNewIndex(new[] { backInst });
            CheckNumberings();
        }     

        private int GetNewIndex()
        {
            if (this.Count == 0)
            {
                return 0;
            }
            else
            {
                return this.Max(x => x.ArgIndex) + 1;
            }
        }

        public void CheckNumberings()
        {
            if (this.Count == 0)
            {
                return;
            }
            if (this.Any(x => x.ArgIndex > MaxArgIndex) && MaxArgIndex != -1)
            {
                throw new Exception("Arg too big detected");
            }
            int maxIndex = this.Max(x => x.ArgIndex);
            for (int i = 0; i <= maxIndex; i++)
            {
                if (!this.Any(x => x.ArgIndex == i))
                {
                    throw new Exception("Index missing");
                }
            }
        }
    }
}
