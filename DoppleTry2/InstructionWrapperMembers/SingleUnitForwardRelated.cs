using System;
using System.Collections.Generic;
using Dopple.InstructionWrapperMembers;

namespace Dopple.InstructionNodes
{
    public class SingleUnitForwardRelated : RelatedList
    {
        public SingleUnitForwardRelated(InstructionNode containingNode) : base(containingNode)
        {
        }

        internal override List<InstructionNode> GetRelatedList(InstructionNode relatedNode)
        {
            return relatedNode.SingleUnitBackRelated;
        }

        internal override RelatedList GetSameList(InstructionNode nodeToMergeInto)
        {
            return nodeToMergeInto.SingleUnitForwardRelated;
        }
    }
}