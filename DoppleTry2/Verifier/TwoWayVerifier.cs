using DoppleTry2.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.VerifierNs
{
    class TwoWayVerifier : Verifier
    {
        public TwoWayVerifier(List<InstructionNode> instructionWrappers) : base(instructionWrappers)
        {

        }

        public override void Verify(InstructionNode instructionWrapper)
        {
            var problematics = instructionWrapper.DataFlowForwardRelated.Where(x => !x.DataFlowBackRelated.Any(y => y.Argument == x)).ToList();
            if (problematics.Count > 0)
            {
               // throw new Exception();
            }

            foreach (var backInst in instructionWrapper.DataFlowBackRelated)
            {
                if (!backInst.Argument.DataFlowForwardRelated.Contains(instructionWrapper))
                {
                    throw new Exception();
                }
            }
            foreach (var forInst in instructionWrapper.DataFlowForwardRelated)
            {
                if (!forInst.DataFlowBackRelated.Select(x => x.Argument).Contains(instructionWrapper))
                {
                    throw new Exception();
                }
            }
        }
    }
}
