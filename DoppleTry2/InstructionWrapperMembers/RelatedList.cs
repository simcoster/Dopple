using DoppleTry2.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.InstructionWrapperMembers
{
    public abstract class RelatedList : List<InstructionNode>
    {
        public RelatedList(InstructionNode containingNode)
        {
            _ContainingNode = containingNode;
        }
        InstructionNode _ContainingNode;
        public void RemoveTwoWay (InstructionNode backArgToRemove)
        {
            base.Remove(backArgToRemove);
            var forwardArg = GetRelatedList(backArgToRemove).First(x => x == _ContainingNode);
            GetRelatedList(backArgToRemove).Remove(forwardArg);
        }

        internal abstract List<InstructionNode> GetRelatedList(InstructionNode backArgToRemove);

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
            base.Add(toAdd);
            GetRelatedList(toAdd).Add(_ContainingNode);
        }
        public void AddTwoWay(IEnumerable<InstructionNode> rangeToAdd)
        {
            foreach (var backArgToAdd in rangeToAdd)
            {
                AddTwoWay(backArgToAdd);
            }
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

    }
}
