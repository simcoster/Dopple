using DoppleTry2.InstructionWrappers;
using DoppleTry2.VerifierNs;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DoppleTry2.VerifierNs
{
    class LdElemVerifier : Verifier
    {
        public LdElemVerifier(List<InstructionWrapper> instructionWrappers) : base(instructionWrappers)
        {
        }

        public override void Verify(InstructionWrapper instructionWrapper)
        {
            if (!CodeGroups.LdElemCodes.Concat(CodeGroups.StElemCodes).Contains(instructionWrapper.Instruction.OpCode.Code))
            {
                return;
            }
            var argumentGroups = instructionWrapper.BackDataFlowRelated.ArgumentList.GroupBy(x => x.ArgIndex).ToList();
            var arrayArgGroup = argumentGroups.First(x => x.SelectMany(y => BacktraceStLdLoc(y.Argument))
                                .All(y => IsProvidingArray(y)));
            argumentGroups.Remove(arrayArgGroup);
            if (argumentGroups.SelectMany(x =>x).All(x => IsProvidingNumber(x.Argument)))
            {
                return;
            }
            else
            {
                throw new Exception("Not all elements are providing numbers");
            }
        }
    }
}
