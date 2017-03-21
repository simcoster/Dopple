using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;

namespace Dopple.InstructionWrapperMembers
{
    public class ProgramFlowBackRoutes : CoupledList
    {
        public ProgramFlowBackRoutes(InstructionNode containingWrapper) : base(containingWrapper)
        {
        }

        internal override List<InstructionNode> GetPartnerList(InstructionNode node)
        {
            return node.ProgramFlowForwardRoutes;
        }

        internal override CoupledList GetSameListInOtherObject(InstructionNode nodeToMergeInto)
        {
            return nodeToMergeInto.ProgramFlowBackRoutes;
        }
    }
}
