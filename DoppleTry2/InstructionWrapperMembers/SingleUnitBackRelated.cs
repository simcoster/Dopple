using System;
using System.Collections.Generic;
using DoppleTry2.InstructionWrapperMembers;

namespace DoppleTry2.InstructionNodes
{
    public class SingleUnitBackRelated : RelatedList
    {
        public SingleUnitBackRelated(InstructionNode containingNode) : base(containingNode)
        {
        }

        internal override List<InstructionNode> GetRelatedList(InstructionNode backArgToRemove)
        {
            return backArgToRemove.SingleUnitForwardRelated;
        }

        internal override RelatedList GetSameList(InstructionNode nodeToMergeInto)
        {
            throw new NotImplementedException();
        }
    }
}