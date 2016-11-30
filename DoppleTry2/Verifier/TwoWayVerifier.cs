using DoppleTry2.InstructionWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.VerifierNs
{
    class TwoWayVerifier : Verifier
    {
        public TwoWayVerifier(List<InstructionWrapper> instructionWrappers) : base(instructionWrappers)
        {

        }

        public override void Verify(InstructionWrapper instructionWrapper)
        {
            return;
            foreach (var backInst in instructionWrapper.BackDataFlowRelated.ArgumentList)
            {
                if (!backInst.Argument.ForwardDataFlowRelated.ArgumentList.Select(x => x.Argument).Contains(instructionWrapper))
                {
                    throw new Exception();
                }
            }
            foreach (var forInst in instructionWrapper.ForwardDataFlowRelated.ArgumentList)
            {
                if (!forInst.Argument.BackDataFlowRelated.ArgumentList.Select(x => x.Argument).Contains(instructionWrapper))
                {
                    throw new Exception();
                }
            }
        }
    }
}
