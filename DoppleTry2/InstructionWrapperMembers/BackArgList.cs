using DoppleTry2.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2
{
    public abstract class BackArgList : List<IndexedArgument>
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

        public static bool SequenceEqualsDeep(IEnumerable<IndexedArgument> firstList, IEnumerable<IndexedArgument> secondList)
        {
            if (firstList.GetType() != secondList.GetType())
            {
                return false;
            }
            return firstList.All(x => secondList.Any(y => y.ArgIndex == x.ArgIndex && x.Argument == y.Argument));
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
            InstructionNode forwardArg = GetForwardList(backArgToRemove.Argument).First(x => x == containingWrapper);
            GetForwardList(backArgToRemove.Argument).Remove(forwardArg);
            if (this.Any(x => !GetForwardList(x.Argument).Contains(containingWrapper)))
            {
                throw new Exception("Validation Failed");
            }
        }
        public void RemoveAllTwoWay(Predicate<IndexedArgument> predicate)
        {
            foreach (var toRemove in this.Where(x => predicate(x)).ToList())
            {
                RemoveTwoWay(toRemove);
            }
        }
        public void RemoveAllTwoWay()
        {
            RemoveAllTwoWay(x => true);
        }
        public void AddTwoWay(IndexedArgument toAdd)
        {
            Add(toAdd);
            GetForwardList(toAdd.Argument).Add(containingWrapper);
        }
        public void AddTwoWay(InstructionNode toAdd)
        {
            var indexedToAdd = new IndexedArgument(GetNewIndex(), toAdd);
            AddTwoWay(indexedToAdd);
        }
        public void AddTwoWay(IEnumerable<IndexedArgument> rangeToAdd)
        {
            foreach (var backArgToAdd in rangeToAdd)
            {
                AddTwoWay(backArgToAdd);
            }
        }

        public void AddTwoWaySingleIndex(IEnumerable<InstructionNode> backInstructions)
        {
            int index = GetNewIndex();
            AddTwoWay(backInstructions.Select(x => new IndexedArgument(index, x)));
        }
        public void AddTwoWay(InstructionNode backInstruction , int index)
        {
            if (this.Any(x => x.ArgIndex == index && x.Argument == backInstruction))
            {
                //TODO to prevent clones
                return;
            }
            AddTwoWay(new IndexedArgument(index, backInstruction));
        }

        public void AddTwoWay(IEnumerable<InstructionNode> instructionNodes, int index)
        {
            foreach(var instWrapper in instructionNodes)
            {
                AddTwoWay(instWrapper, index);
            }
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
                    //TODO remove
                    //throw new Exception("Index missing");
                }
            }
        }

        protected abstract List<InstructionNode> GetForwardList(InstructionNode node);
    }

    public class DataFlowBackArgList : BackArgList
    {
        public DataFlowBackArgList(InstructionNode instructionWrapper) : base(instructionWrapper)
        {
        }

        protected override List<InstructionNode> GetForwardList(InstructionNode node)
        {
            return node.DataFlowForwardRelated;
        }
    }

    public class ProgramFlowAffectedBackArgList : BackArgList
    {
        public ProgramFlowAffectedBackArgList(InstructionNode instructionWrapper) : base(instructionWrapper)
        {
        }

        protected override List<InstructionNode> GetForwardList(InstructionNode node)
        {
            return node.ProgramFlowForwardAffecting;
        }
    }
}
