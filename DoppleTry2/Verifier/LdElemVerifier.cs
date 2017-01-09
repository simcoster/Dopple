using DoppleTry2.VerifierNs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionNodes;

namespace DoppleTry2
{
    class LdElemVerifier : Verifier
    {
        public LdElemVerifier(List<InstructionNode> instructionWrappers) : base(instructionWrappers)
        {
        }

        public override void Verify(InstructionNode instructionNode)
        {
            if (!CodeGroups.LdElemCodes.Contains(instructionNode.Instruction.OpCode.Code))
            {
                return;
            }
            var stElemOptionalArgs = instructionNode.DataFlowBackRelated.Where(x => x.ArgIndex == 2);
            if (!stElemOptionalArgs.All(x => CodeGroups.StElemCodes.Contains(x.Argument.Instruction.OpCode.Code)))
            {
                throw new Exception("Bad Stelem argument");
            }
            var arrayRefArgs = instructionNode.DataFlowBackRelated.Where(x => x.ArgIndex == 1);
            if (!arrayRefArgs.All(x => IsProvidingArray(x.Argument)))
            {
                throw new Exception("Bad array ref argument");
            }
            var arrayIndexArgs = instructionNode.DataFlowBackRelated.Where(x => x.ArgIndex == 0);
            if (!arrayIndexArgs.All(x => IsProvidingNumber(x.Argument)))
            {
                throw new Exception("Bad array index argument");
            }
            if (instructionNode.DataFlowBackRelated.Max(x => x.ArgIndex) > 2)
            {
                throw new Exception("too many arguments!");
            }
        }
    }
}
