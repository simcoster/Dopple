using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace Dopple.BackTrackers
{
    class InlinedNewObjBackTracer : SingeIndexBackTracer
    {
        public InlinedNewObjBackTracer(List<InstructionNode> instructionNodes) : base(instructionNodes)
        {
        }

        public override Code[] HandlesCodes
        {
            get
            {
                return new[] { Code.Newobj };
            }
        }

        protected override IEnumerable<InstructionNode> GetDataflowBackRelatedArgGroup(InstructionNode currentInst)
        {
            if (!currentInst.InliningProperties.Inlined)
            {
                return new InstructionNode[0];
            }
            NewObjectNode newObjectNode = (NewObjectNode) currentInst;
            IEnumerable<InlineableCallNode> constructorCalls = _SingleIndexBackSearcher.SearchBackwardsForDataflowInstrcutions(x => x is InlineableCallNode && ((InlineableCallNode) x).TargetMethod.FullName == ((MethodReference) currentInst.Instruction.Operand).FullName, currentInst).Cast<InlineableCallNode>();
            if (constructorCalls.Count() != 1)
            {
                throw new Exception("Too many call nodes");
            }
            var constructorCall = constructorCalls.First();
            if (constructorCall.DataFlowBackRelated.Count != newObjectNode.ConstructorParamCount)
            {
                throw new Exception("Not enough parameters");
            }
            return constructorCall.DataFlowBackRelated.Select(x => x.Argument);
        }
    }
}
