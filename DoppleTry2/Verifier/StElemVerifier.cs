using Dopple.InstructionNodes;
using Dopple.VerifierNs;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dopple.VerifierNs
{
    class StElemVerifier : Verifier
    {
        public StElemVerifier(List<InstructionNode> instructionNodes) : base(instructionNodes)
        {
        }

        public override void Verify(InstructionNode instructionWrapper)
        {
            if (!CodeGroups.StElemCodes.Contains(instructionWrapper.Instruction.OpCode.Code))
            {
                return;
            }
            var ldArgGroup = instructionWrapper.DataFlowBackRelated.Where(x => x.ArgIndex == 0);
            if (!ldArgGroup.All(x => IsProvidingArray(x.Argument)))
            {
                throw new Exception("Bad array reference argument");
            }
            var locationArgGroup = instructionWrapper.DataFlowBackRelated.Where(x => x.ArgIndex == 1);
            if (!locationArgGroup.All(x => IsProvidingNumber(x.Argument)))
            {
                throw new Exception("Bad array location argument");
            }
            var valueArgGroup = instructionWrapper.DataFlowBackRelated.Where(x => x.ArgIndex == 2);
            if (!locationArgGroup.All(x => IsProvidingNumber(x.Argument)))
            {
                throw new Exception("Bad value argument");
            }
            if (instructionWrapper.DataFlowBackRelated.Max(x => x.ArgIndex) > 2)
            {
                throw new Exception("too many arguments!");
            }
        }
    }
}
