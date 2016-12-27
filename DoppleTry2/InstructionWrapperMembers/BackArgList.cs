using DoppleTry2.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2
{
    public class BackArgList : List<IndexedArgument>
    {
        public BackArgList(InstructionNode instructionWrapper)
        {
            containingWrapper = instructionWrapper;
        }
        public override bool Equals(object obj)
        {
            if (obj is BackArgList)
            {
                return this.All(x => ((BackArgList)obj).Any(y => y.ArgIndex == x.ArgIndex && y.Argument == x.Argument));
            }
            return base.Equals(obj);
        }

        readonly InstructionNode containingWrapper;
        public int MaxArgIndex = -1;

        public bool SelfFeeding
        {
            get
            {
                return this.Any(x => x.Argument == containingWrapper);
            }
        }

        [Obsolete("Use remove 2 way instead")]
        public new bool Remove(IndexedArgument item)
        {
            return base.Remove(item);
        }
        public void RemoveTwoWay(IndexedArgument backArgToRemove)
        {
            Remove(backArgToRemove);
            InstructionNode forwardArg = backArgToRemove.Argument.DataFlowForwardRelated.First(x => x == containingWrapper);
            backArgToRemove.Argument.DataFlowForwardRelated.Remove(forwardArg);
            if (this.Any(x => !x.Argument.DataFlowForwardRelated.Contains(containingWrapper)))
            {
                throw new Exception("Validation Failed");
            }
        }
        public void RemoveAllTwoWay(Predicate<IndexedArgument> predicate)
        {
            foreach (var toRemove in this.Where(x => predicate(x)))
            {
                RemoveTwoWay(toRemove);
            }
        }
        public void AddTwoWay(IndexedArgument toAdd)
        {
            Add(toAdd);
            toAdd.Argument.DataFlowForwardRelated.Add(containingWrapper);
        }
        public void AddTwoWay(InstructionNode toAdd)
        {
            var indexedToAdd = new IndexedArgument(GetNewIndex(), toAdd);
            AddTwoWay(indexedToAdd);
        }
        public void AddRangeTwoWay(IEnumerable<IndexedArgument> rangeToAdd)
        {
            foreach (var backArgToAdd in rangeToAdd)
            {
                AddTwoWay(backArgToAdd);
            }
        }

        public void AddWithNewIndex(IEnumerable<InstructionNode> backInstructions)
        {
            int index = GetNewIndex();
            AddRangeTwoWay(backInstructions.Select(x => new IndexedArgument(index, x)));
            CheckNumberings();
        }
        public void AddWithNewIndexes(IEnumerable<InstructionNode> instructionWrappers)
        {
            foreach (var instWrapper in instructionWrappers)
            {
                AddWithNewIndex(instWrapper);
            }
        }
        public void AddWithExistingIndex(InstructionNode backInstruction , int index)
        {
            if (this.Any(x => x.ArgIndex == index && x.Argument == backInstruction))
            {
                return;
            }
            AddTwoWay(new IndexedArgument(index, backInstruction));
            CheckNumberings();
        }
        public void AddWithExistingIndex(IndexedArgument indexedArg)
        {
            AddWithExistingIndex(indexedArg.Argument,indexedArg.ArgIndex);
        }
        public void AddMultipleWithExistingIndex(IEnumerable<InstructionNode> instructionWrappers, int index)
        {
            foreach(var instWrapper in instructionWrappers)
            {
                AddWithExistingIndex(instWrapper, index);
            }
        }       

        public void AddPreserveIndexes(BackArgList argList)
        {
            AddRangeTwoWay(argList);
            CheckNumberings();
        }
        
        public void AddWithNewIndex(InstructionNode backInst)
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
