using DoppleTry2.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.InstructionWrapperMembers
{
    public class ProgramFlowBackAffected : ArgList
    {
        public ProgramFlowBackAffected(InstructionNode containingWrapper) : base(containingWrapper)
        {
        }

        protected override ArgList GetMirrorList(InstructionNode node)
        {
            return node.ProgramFlowForwardAffecting;
        }

        internal override ArgList GetSameList(InstructionNode nodeToMergeInto)
        {
            throw new NotImplementedException();
        }
    }
}
