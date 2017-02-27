using Dopple.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.InstructionWrapperMembers
{
    public class ProgramFlowBackAffected : CoupledIndexedArgList
    {
        public ProgramFlowBackAffected(InstructionNode containingWrapper) : base(containingWrapper)
        {
        }

        protected override CoupledIndexedArgList GetMirrorList(InstructionNode node)
        {
            return node.ProgramFlowForwardAffecting;
        }

        internal override CoupledIndexedArgList GetSameList(InstructionNode nodeToMergeInto)
        {
            throw new NotImplementedException();
        }
    }
}
