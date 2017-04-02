using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;

namespace Dopple.VerifierNs
{
    class ArgRemovedVerifier : Verifier
    {
        public ArgRemovedVerifier(List<InstructionNode> instructionNodes) : base(instructionNodes)
        {
        }

        public override void Verify(InstructionNode instructionNode)
        {
            var allRelated = instructionNode.DataFlowBackRelated.Concat(instructionNode.DataFlowForwardRelated);
            var removedRelated = allRelated.Where(x => !instructionNodes.Contains(x.Argument)).ToList();
            if (removedRelated.Any())
            {
                var removed = instructionNode.DataFlowBackRelated.Where(x => !instructionNodes.Contains(x.Argument)).ToList();
                var removed2 = instructionNode.DataFlowForwardRelated.Where(x => !instructionNodes.Contains(x.Argument)).ToList();

                throw new NotImplementedException();
            }
            
        }
    }
}
