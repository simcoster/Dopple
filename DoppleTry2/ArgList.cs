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

        public void AddSingleIndex(IEnumerable<InstructionWrapper> instructionWrappers)
        {
            ArgumentList.AddRange(instructionWrappers.Select(x => new IndexedArgument(GetNewIndex() + 1, x)));
        }
        public void AddSingleIndex(ArgList argList, int index =-1)
        {
            if (index == -1)
            {
                ArgumentList.AddRange(argList.ArgumentList.Select(x => new IndexedArgument(GetNewIndex() + 1, x.Argument)).ToArray());
            }
            else
            {
                ArgumentList.AddRange(argList.ArgumentList.Select(x => new IndexedArgument(index, x.Argument)).ToArray());
            }
        }
        public void AddSingleIndex(InstructionWrapper instructionWrapper)
        {
            AddSingleIndex(new[] { instructionWrapper });
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
