using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionNodes;

namespace DoppleTry2.InstructionWrapperMembers
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

       
    }

}
