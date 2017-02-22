using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;

namespace Dopple.InstructionWrapperMembers
{
    public class ProgramFlowForwardRoutes : RelatedList
    {
        public ProgramFlowForwardRoutes(InstructionNode containingWrapper) : base(containingWrapper)
        {
        }

        internal override List<InstructionNode> GetRelatedList(InstructionNode backArgToRemove)
        {
            return backArgToRemove.ProgramFlowBackRoutes;
        }

        internal override RelatedList GetSameList(InstructionNode nodeToMergeInto)
        {
            return nodeToMergeInto.ProgramFlowForwardRoutes;
        }
    }

}
