using DoppleTry2.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.InstructionWrapperMembers
{
    public class ProgramFlowBackAffected : RelatedList
    {
        public ProgramFlowBackAffected(InstructionNode containingWrapper) : base(containingWrapper)
        {
        }

        internal override List<InstructionNode> GetRelatedList(InstructionNode node)
        {
            return node.ProgramFlowForwardAffecting;
        }
    }
}
