using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;
using Mono.Cecil.Cil;

namespace Dopple.BackTracers
{
    class SingleConditionOperationUnitBackTracer : BackTracer
    {
        private Code[] InUnitCodes = CodeGroups.LdLocCodes
                                    .Concat(CodeGroups.StLocCodes)
                                    .Concat(new []{ Code.And, Code.Or, Code.Ceq, Code.Cgt, Code.Cgt_Un, Code.Clt, Code.Clt_Un }).ToArray();
        public override Code[] HandlesCodes
        {
            get
            {
                return new[] { Code.Brfalse, Code.Brtrue, Code.Brfalse_S, Code.Brtrue_S };
            }
        }

        protected override void InnerAddBackDataflowConnections(InstructionNode currentInst)
        {
            Queue<InstructionNode> toCheck = new Queue<InstructionNode>();
            toCheck.Enqueue(currentInst);
            while (toCheck.Count >0)
            {
                var currentChecked = toCheck.Dequeue();
                var backParticipating = currentChecked.DataFlowBackRelated.Where(x => InUnitCodes.Contains(x.Argument.Instruction.OpCode.Code));
                foreach(var backNode in backParticipating)
                {
                    currentChecked.SingleUnitBackRelated.AddTwoWay(backNode.Argument);
                    toCheck.Enqueue(backNode.Argument);
                }
            }
        }
    }
}
