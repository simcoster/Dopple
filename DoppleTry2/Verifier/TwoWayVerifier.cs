using Dopple.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.VerifierNs
{
    class TwoWayVerifier : Verifier
    {
        public TwoWayVerifier(List<InstructionNode> instructionNodes) : base(instructionNodes)
        {
        }

        public override void Verify(InstructionNode instructionNode)
        {
            var problematics = instructionNode.DataFlowForwardRelated.Where(x => !x.Argument.DataFlowBackRelated.Any(y => y.Argument == x.Argument)).ToList();
            if (problematics.Count > 0)
            {
               // throw new Exception();
            }

            foreach (var backInst in instructionNode.DataFlowBackRelated)
            {
                if (!backInst.Argument.DataFlowForwardRelated.Any(x => x.Argument ==instructionNode))
                {
                    throw new Exception();
                }
            }
            foreach (var forInst in instructionNode.DataFlowForwardRelated)
            {
                if (!forInst.Argument.DataFlowBackRelated.Select(x => x.Argument).Contains(instructionNode))
                {
                    throw new Exception();
                }
            }
        }
    }
}
