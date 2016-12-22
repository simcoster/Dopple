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
            var problematics = instructionWrapper.ForwardDataFlowRelated.Where(x => !x.BackDataFlowRelated.Any(y => y.Argument == x)).ToList();
            if (problematics.Count > 0)
            {
               // throw new Exception();
            }

            foreach (var backInst in instructionWrapper.BackDataFlowRelated)
            {
                if (!backInst.Argument.ForwardDataFlowRelated.Contains(instructionWrapper))
                {
                    throw new Exception();
                }
            }
            foreach (var forInst in instructionWrapper.ForwardDataFlowRelated)
            {
                if (!forInst.BackDataFlowRelated.Select(x => x.Argument).Contains(instructionWrapper))
                {
                    throw new Exception();
                }
            }
        }
    }
}
