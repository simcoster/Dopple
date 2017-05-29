using System;
using System.Collections.Generic;
using Dopple.InstructionWrapperMembers;

namespace Dopple.InstructionNodes
{
    public class SingleUnitBackRelated : CoupledList
    {
        public SingleUnitBackRelated(InstructionNode containingNode) : base(containingNode)
        {
        }

        internal override CoupledList GetPartnerList(InstructionNode backArgToRemove)
        {
            return backArgToRemove.SingleUnitForwardRelated;
        }

        internal override CoupledList GetSameListInOtherObject(InstructionNode nodeToMergeInto)
        {
            return nodeToMergeInto.SingleUnitBackRelated;
        }
    }
}