using Dopple.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.InstructionWrapperMembers
{
    public abstract class CoupledList : List<InstructionNode>, IMergable
    {
        public CoupledList(InstructionNode containingNode)
        {
            _ContainingNode = containingNode;
        }
        InstructionNode _ContainingNode;
        public void RemoveTwoWay (InstructionNode backArgToRemove)
        {
            Remove(backArgToRemove);
            var forwardArg = GetPartnerList(backArgToRemove).First(x => x == _ContainingNode);
            GetPartnerList(backArgToRemove).Remove(forwardArg);
        }

        internal abstract List<InstructionNode> GetPartnerList(InstructionNode backArgToRemove);

        public void RemoveAllTwoWay (Predicate<InstructionNode> predicate)
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

        public void AddTwoWay(InstructionNode toAdd)
        {
            if (Contains(toAdd))
            {
                //No duplicities allowed
                return;
            }
            base.Add(toAdd);
            GetPartnerList(toAdd).Add(_ContainingNode);
        }
        public void AddTwoWay(IEnumerable<InstructionNode> rangeToAdd)
        {
            foreach (var backArgToAdd in rangeToAdd.ToArray())
            {
                AddTwoWay(backArgToAdd);
            }
        }

        public IEnumerable<InstructionNode> GetBackTree()
        {
            return this.SelectMany(x => GetSameListInOtherObject(x).GetBackTree().Concat(new[] { x }));
        }

        [Obsolete ("Please use AddTwoWay instead")]
        public new void Add(InstructionNode instructionWrapper)
        {
            base.Add(instructionWrapper);
        }


        [Obsolete("Please use AddRangeTwoWay instead")]
        public new void AddRange(IEnumerable<InstructionNode> instructionWrappers)
        {
            base.AddRange(instructionWrappers);
        }

        [Obsolete("Please use RemoveTwoWay instead")]
        public new void Remove(InstructionNode instructionWrapper)
        {
            base.Remove(instructionWrapper);
        }

        [Obsolete("Please use RemoveAllTwoWay instead")]
        public new void RemoveAll(Predicate<InstructionNode> predicate)
        {
            base.RemoveAll(predicate);
        }

        internal abstract CoupledList GetSameListInOtherObject(InstructionNode nodeToMergeInto);

        public void MergeInto(InstructionNode nodeToMergeInto, bool KeepOriginal)
        {
            foreach (var arg in this.ToArray())
            {
                if (!KeepOriginal)
                {
                    this.RemoveTwoWay(arg);
                }
                GetSameListInOtherObject(nodeToMergeInto).AddTwoWay(arg);
            }
        }
    }
}
