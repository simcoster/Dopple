﻿using System;
using System.Collections.Generic;
using DoppleTry2.InstructionWrapperMembers;

namespace DoppleTry2.InstructionNodes
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