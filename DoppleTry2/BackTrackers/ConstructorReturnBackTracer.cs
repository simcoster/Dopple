
using System;
using System.Collections.Generic;
using Dopple.InstructionNodes;
using Mono.Cecil.Cil;
using System.Linq;

namespace Dopple.BackTrackers
{
    public class ConstructorReturnBackTracer : SingeIndexBackTracer
    {
        public ConstructorReturnBackTracer(List<InstructionNode> instructionNodes) : base(instructionNodes)
        {
        }

        public override Code[] HandlesCodes
        {
            get
            {
                return new[] { Code.Ret };
            }
        }

        protected override IEnumerable<InstructionNode> GetDataflowBackRelatedArgGroup(InstructionNode instructionNode)
        {
            if (!instructionNode.Method.IsConstructor)
            {
                return new InstructionNode[0];
            }
            var constructorCalls = _SingleIndexBackSearcher.SearchBackwardsForDataflowInstrcutions(x => x is ConstructorCallNode && ((ConstructorCallNode) x).CalledFunction == instructionNode.Method, instructionNode);
            if (constructorCalls.Count > 1)
            {
                throw new Exception("Should only be one");
            }
            return constructorCalls.First().DataFlowBackRelated.Where(x => x.ArgIndex == 0).Select(x => x.Argument);
        }
    }
}
