using DoppleTry2.InstructionWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2
{
    public class ArgList
    {
        public List<IndexedArgument> ArgumentList { get; internal set; } = new List<IndexedArgument>();

        public void AddWithNewIndex(IEnumerable<InstructionWrapper> instructionWrappers)
        {
            int index = GetNewIndex();
            ArgumentList.AddRange(instructionWrappers.Select(x => new IndexedArgument(index, x)));
            CheckNumberings();
        }
        public void AddWithNewIndexes(IEnumerable<InstructionWrapper> instructionWrappers)
        {
            foreach (var instWrapper in instructionWrappers)
            {
                AddWithNewIndex(instWrapper);
            }
        }
        public void AddWithExistingIndex(InstructionWrapper instructionWrapper , int index)
        {
            ArgumentList.Add(new IndexedArgument(index, instructionWrapper));
            CheckNumberings();
        }
        public void AddWithExistingIndex(IndexedArgument indexedArg)
        {
            AddWithExistingIndex(indexedArg.Argument,indexedArg.ArgIndex);
        }
        public void AddWithExistingIndex(IEnumerable<InstructionWrapper> instructionWrappers, int index)
        {
            foreach(var instWrapper in instructionWrappers)
            {
                AddWithExistingIndex(instWrapper, index);
            }
        }

        internal void AddWithExistingIndex(List<IndexedArgument> argumentList)
        {
            foreach(var indexedArg in argumentList)
            {
                AddWithExistingIndex(indexedArg);
            }
        }

        public void AddWithNewIndex(ArgList argList, int index =-1)
        {
            if (index == -1)
            {
                ArgumentList.AddRange(argList.ArgumentList.Select(x => new IndexedArgument(GetNewIndex(), x.Argument)).ToArray());
            }
            else
            {
                ArgumentList.AddRange(argList.ArgumentList.Select(x => new IndexedArgument(index, x.Argument)).ToArray());
            }
            CheckNumberings();
        }

        public void AddPreserveIndexes(ArgList argList)
        {
            ArgumentList.AddRange(argList.ArgumentList);
            CheckNumberings();
        }
        public void AddWithNewIndex(InstructionWrapper instructionWrapper)
        {
            AddWithNewIndex(new[] { instructionWrapper });
            CheckNumberings();
        }

        public override bool Equals (object otherObject)
        {
            if (otherObject is ArgList)
            {
                return ArgumentList.SequenceEqual(((ArgList)otherObject).ArgumentList);
            }
            else
            {
                return base.Equals(otherObject);
            }
        }

        private int GetNewIndex()
        {
            if (ArgumentList.Count == 0)
            {
                return 0;
            }
            else
            {
                return ArgumentList.Max(x => x.ArgIndex) + 1;
            }
        }

        public void CheckNumberings()
        {
            if (ArgumentList.Count == 0)
            {
                return;
            }
            int maxIndex = ArgumentList.Max(x => x.ArgIndex);
            for (int i = 0; i <= maxIndex; i++)
            {
                if (!ArgumentList.Any(x => x.ArgIndex == i))
                {
                    throw new Exception("Index missing");
                }
            }
        }
    }
}
