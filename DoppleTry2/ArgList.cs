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
        }
        public void AddWithExistingIndex(IEnumerable<InstructionWrapper> instructionWrappers, int index)
        {
            foreach(var instWrapper in instructionWrappers)
            {
                AddWithExistingIndex(instWrapper, index);
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
        }

        public void AddPreserveIndexes(ArgList argList)
        {
            ArgumentList.AddRange(argList.ArgumentList);
        }
        public void AddWithNewIndex(InstructionWrapper instructionWrapper)
        {
            AddWithNewIndex(new[] { instructionWrapper });
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
    }

}
