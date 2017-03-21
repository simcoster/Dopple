using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;

namespace Dopple.InstructionWrapperMembers
{
    public class ProgramFlowForwardRoutes : CoupledList
    {
        public ProgramFlowForwardRoutes(InstructionNode containingWrapper) : base(containingWrapper)
        {
        }

        internal override List<InstructionNode> GetPartnerList(InstructionNode backArgToRemove)
        {
            return backArgToRemove.ProgramFlowBackRoutes;
        }

        internal override CoupledList GetSameListInOtherObject(InstructionNode nodeToMergeInto)
        {
            return nodeToMergeInto.ProgramFlowForwardRoutes;
        }
    }

}
