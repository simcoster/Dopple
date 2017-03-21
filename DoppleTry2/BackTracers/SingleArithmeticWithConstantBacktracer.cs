using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;
using Mono.Cecil.Cil;

namespace Dopple.BackTracers
{
    class SingleArithmeticWithConstantBacktracer : BackTracer
    {
        public override Code[] HandlesCodes
        {
            get
            {
                return CodeGroups.ArithmeticCodes;
            }
        }

        public override void BackTraceDataFlowSingle(InstructionNode currentInst)
        {
            if (currentInst.DataFlowBackRelated.SelfFeeding)
            {
                return;
            }
            var constValueArgGroups = currentInst.DataFlowBackRelated.GroupBy(x => x.ArgIndex).Where(x => x.All(y => y.Argument is LdImmediateInstNode)).ToList();
            if (constValueArgGroups.Count != 1)
            {
                return;
            }
            foreach(var nonConstantArg in currentInst.DataFlowBackRelated.Where(x => x.ArgIndex != constValueArgGroups[0].First().ArgIndex))
            {
                nonConstantArg.Argument.SingleUnitBackRelated.AddTwoWay(currentInst);
            }
            foreach(var constatArg in constValueArgGroups.First())
            {
                currentInst.SingleUnitBackRelated.AddTwoWay(constatArg.Argument);
            }
        }
    }
}
