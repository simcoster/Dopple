using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using Dopple.InstructionNodes;

namespace Dopple.BackTracers
{
    class LdArgBacktracer : SingeIndexBackTracer
    {
        protected override IEnumerable<InstructionNode> GetDataflowBackRelatedArgGroup(InstructionNode instNode)
        {
            if (instNode.InliningProperties.Inlined)
            {
                InlineableCallNode inlinedCall = instNode.InliningProperties.CallNode;
                var argSuppliers = new List<InstructionNode>();
                argSuppliers.AddRange(inlinedCall.DataFlowBackRelated.Where(x => x.ArgIndex == ((LdArgInstructionNode) instNode).ArgIndex).Select(x => x.Argument));
                return argSuppliers;          
            }
            else
            {
                return SingleIndexBackSearcher.SafeSearchBackwardsForDataflowInstrcutions(x => x is StArgInstructionNode && ((StArgInstructionNode)x).ArgIndex == ((LdArgInstructionNode)instNode).ArgIndex, instNode);
            }
        }

        public override Code[] HandlesCodes => CodeGroups.LdArgCodes; 
    }
}
