﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using Dopple.InstructionNodes;

namespace Dopple.BackTrackers
{
    class LdArgBacktracer : SingeIndexBackTracer
    {
        public LdArgBacktracer(List<InstructionNode> instructionNodes) : base(instructionNodes)
        {
        }
        protected override IEnumerable<InstructionNode> GetDataflowBackRelatedArgGroup(InstructionNode instNode)
        {
            if (instNode.InliningProperties.Inlined)
            {
                IEnumerable<InlineableCallNode> inlinedCalls = _SingleIndexBackSearcher.SearchBackwardsForDataflowInstrcutions(x => x is InlineableCallNode && ((InlineableCallNode)x).TargetMethod.FullName == instNode.Method.FullName,instNode).Cast<InlineableCallNode>();
                var argSuppliers = new List<InstructionNode>();
                foreach(var inlinedCall in inlinedCalls)
                {
                    argSuppliers.AddRange(inlinedCall.DataFlowBackRelated.Where(x => x.ArgIndex == ((LdArgInstructionNode) instNode).ArgIndex).Select(x => x.Argument));
                }
                return argSuppliers;          
            }
            else
            {
                return _SingleIndexBackSearcher.SafeSearchBackwardsForDataflowInstrcutions(x => x is StArgInstructionNode && ((StArgInstructionNode)x).ArgIndex == ((LdArgInstructionNode)instNode).ArgIndex, instNode);
            }
        }

        public override Code[] HandlesCodes => CodeGroups.LdArgCodes; 
    }
}
