using System;
using System.Collections.Generic;
using Dopple.InstructionWrapperMembers;

namespace Dopple.InstructionNodes
{
    public class SingleUnitForwardRelated : CoupledList
    {
        public SingleUnitForwardRelated(InstructionNode containingNode) : base(containingNode)
        {
        }

        internal override CoupledList GetPartnerList(InstructionNode relatedNode)
        {
            return relatedNode.SingleUnitBackRelated;
        }

        internal override CoupledList GetSameListInOtherObject(InstructionNode nodeToMergeInto)
        {
            return nodeToMergeInto.SingleUnitForwardRelated;
        }
    }
}