using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionNodes;

namespace DoppleTry2.InstructionWrapperMembers
{
    public class ProgramFlowBackRoutes : RelatedList
    {
        public ProgramFlowBackRoutes(InstructionNode containingWrapper) : base(containingWrapper)
        {
        }

        internal override List<InstructionNode> GetRelatedList(InstructionNode node)
        {
            return node.ProgramFlowForwardRoutes;
        }
    }
}
