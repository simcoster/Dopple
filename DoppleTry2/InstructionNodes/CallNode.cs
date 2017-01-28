using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DoppleTry2.InstructionNodes
{
    public abstract class CallNode : InstructionNode
    {
        private CallDataFlowBackArgList _CallDataFlowBackArgList;
        public CallNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            TargetMethod = (MethodReference) instruction.Operand;
            StackPopCount = TargetMethod.Parameters.Count;
            _CallDataFlowBackArgList = new CallDataFlowBackArgList(this);

        }
        public override DataFlowBackArgList DataFlowBackRelated
        {
            get
            {
                return _CallDataFlowBackArgList;
            }
        }

        public MethodReference TargetMethod { get; private set; }
    }
}
