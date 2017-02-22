using System;
using System.Collections.Generic;
using Dopple.InstructionWrapperMembers;

namespace Dopple.InstructionNodes
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
            return nodeToMergeInto.SingleUnitBackRelated;
        }
    }
}