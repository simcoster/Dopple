using Dopple.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Dopple
{
    [CollectionDataContract]
    public abstract class CoupledIndexedArgList : List<IndexedArgument>, IMergable
    {
        public CoupledIndexedArgList() {}
        public CoupledIndexedArgList(InstructionNode instructionNode)
        {
            containingNode = instructionNode;
        }
        public override bool Equals(object obj)
        {
            if (obj is CoupledIndexedArgList)
            {
                return this.All(x => ((CoupledIndexedArgList)obj).Any(y => y.ArgIndex == x.ArgIndex && y.Argument == x.Argument));
            }
            return base.Equals(obj);
        }

        public static bool SequenceEqualsWithIndexes(IEnumerable<IndexedArgument> firstList, IEnumerable<IndexedArgument> secondList)
        {
            if (firstList.GetType() != secondList.GetType())
            {
                return false;
            }
            List<IndexedArgument> firstListCopy = new List<IndexedArgument>(firstList);
            List<IndexedArgument> secondListCopy = new List<IndexedArgument>(secondList);
            foreach(var indexedArg in firstList)
            {
                var secondListMatch = secondListCopy.FirstOrDefault(x => x.Argument == indexedArg.Argument && x.ArgIndex == indexedArg.ArgIndex);
                if (secondListMatch == null)
                {
                    return false;
                }
                else
                {
                    firstListCopy.Remove(indexedArg);
                    secondListCopy.Remove(secondListMatch);
                }
            }
            if (secondListCopy.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        protected readonly InstructionNode containingNode;
        public int MaxArgIndex = -1;

        public bool SelfFeeding
        {
            get
            {
                return this.Any(x => x.Argument == containingNode);
            }
        }

        [Obsolete("Use remove 2 way instead")]
        public new bool Remove(IndexedArgument item)
        {
            return base.Remove(item);
        }
        public void RemoveTwoWay(IndexedArgument argToRemove)
        {
            Remove(argToRemove);
            var forwardArg = argToRemove.MirrorArg;
            GetMirrorList(argToRemove.Argument).Remove(forwardArg);
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
        virtual public void AddTwoWay(IndexedArgument toAdd)
        {
            if (this.Any(x => x.ArgIndex==toAdd.ArgIndex && x.Argument == toAdd.Argument))
            {
                return;
            }
            var toAddClone = new IndexedArgument(toAdd.ArgIndex, toAdd.Argument, toAdd.ContainingList);
            Add(toAddClone);
            var mirrorList = GetMirrorList(toAddClone.Argument);
            var mirrorArg = new IndexedArgument(toAddClone.ArgIndex, containingNode, mirrorList);
            mirrorArg.MirrorArg = toAddClone;
            toAddClone.MirrorArg = mirrorArg;
            mirrorList.Add(mirrorArg);
        }
      
        public void AddTwoWay(IEnumerable<IndexedArgument> rangeToAdd)
        {
            foreach (var backArgToAdd in rangeToAdd)
            {
                AddTwoWay(backArgToAdd);
            }
        }

     
        public void AddTwoWay(InstructionNode backInstruction , int index)
        {
            AddTwoWay(new IndexedArgument(index, backInstruction,this));
        }

        public void AddTwoWay(IEnumerable<InstructionNode> instructionNodes, int index)
        {
            foreach(var instWrapper in instructionNodes)
            {
                AddTwoWay(instWrapper, index);
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
        internal abstract CoupledIndexedArgList GetSameList(InstructionNode nodeToMergeInto);
        protected abstract CoupledIndexedArgList GetMirrorList(InstructionNode node);

        public void MergeInto(InstructionNode nodeToMergeInto, bool KeepOriginal)
        {
            CoupledIndexedArgList mergedNodeSameArgList = GetSameList(nodeToMergeInto);
            foreach (var arg in this.ToArray())
            {
                mergedNodeSameArgList.AddTwoWay(arg);
                if (!KeepOriginal)
                {
                    RemoveTwoWay(arg);
                }
            }
        }
    }

    public class DataFlowBackArgList : CoupledIndexedArgList
    {
        protected int CurrentIndex;
        public DataFlowBackArgList(InstructionNode instructionWrapper) : base(instructionWrapper)
        {
            ResetIndex();
        }

        public void ResetIndex()
        {
            CurrentIndex = containingNode.StackPopCount - 1;
        }

        public virtual void AddTwoWaySingleIndex(IEnumerable<InstructionNode> backInstructions)
        {
            int index = CurrentIndex + 1;
            AddTwoWay(backInstructions.Select(x => new IndexedArgument(index, x, this)));
        }

        public virtual void AddTwoWay(InstructionNode toAdd)
        {
            if (CurrentIndex < 0)
            {
                throw new Exception("Current Index should be 0 or larger");
            }
            var indexedToAdd = new IndexedArgument(CurrentIndex, toAdd, this);
            AddTwoWay(indexedToAdd);
            CurrentIndex--;
        }

        protected override CoupledIndexedArgList GetMirrorList(InstructionNode node)
        {
            return node.DataFlowForwardRelated;
        }

        internal override CoupledIndexedArgList GetSameList(InstructionNode nodeToMergeInto)
        {
            return nodeToMergeInto.DataFlowBackRelated;
        }      

        internal void UpdateLargestIndex()
        {
            if (this.Count > 0)
            {
                CurrentIndex = this.Max(x => x.ArgIndex);
            }
        }

        internal void AddTwoWayWithNewIndex(InstructionNode node)
        {
            int newIndex = this.Count == 0?  0  : this.Max(x => x.ArgIndex) + 1;
            base.AddTwoWay(node, newIndex);
        }
    }

 
    public class DataFlowForwardArgList : CoupledIndexedArgList
    {
        private DataFlowForwardArgList() { }
        public DataFlowForwardArgList(InstructionNode instructionWrapper) : base(instructionWrapper)
        {
        }

        protected override CoupledIndexedArgList GetMirrorList(InstructionNode node)
        {
            return node.DataFlowBackRelated;
        }

        internal override CoupledIndexedArgList GetSameList(InstructionNode nodeToMergeInto)
        {
            return nodeToMergeInto.DataFlowForwardRelated;
        }
    }

    public class ProgramFlowBackAffectedArgList : CoupledIndexedArgList
    {
        public ProgramFlowBackAffectedArgList(InstructionNode instructionWrapper) : base(instructionWrapper)
        {
        }

        protected override CoupledIndexedArgList GetMirrorList(InstructionNode node)
        {
            return node.ProgramFlowForwardAffecting;
        }

        internal override CoupledIndexedArgList GetSameList(InstructionNode nodeToMergeInto)
        {
            return nodeToMergeInto.ProgramFlowBackAffected;
        }
    }

    public class ProgramFlowForwardAffectingArgList : CoupledIndexedArgList
    {
        public ProgramFlowForwardAffectingArgList(InstructionNode instructionWrapper) : base(instructionWrapper)
        {
        }

        protected override CoupledIndexedArgList GetMirrorList(InstructionNode node)
        {
            return node.ProgramFlowBackAffected;
        }

        internal override CoupledIndexedArgList GetSameList(InstructionNode nodeToMergeInto)
        {
            return nodeToMergeInto.ProgramFlowForwardAffecting;
        }
    }
}
