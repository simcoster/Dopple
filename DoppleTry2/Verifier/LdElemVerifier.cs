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
            var arrayArgGroup = argumentGroups.Where(x => x.SelectMany(y => BacktraceStLdLoc(y.Argument))
                                .All(y => IsProvidingArray(y)));
            if (arrayArgGroup.Count() < 1)
            {
                throw new Exception("No array reference argument");
            }
            argumentGroups = argumentGroups.Except(arrayArgGroup).ToList();
            if (argumentGroups.SelectMany(x =>x).SelectMany(x => BacktraceStLdLoc(x.Argument)).All(x => IsProvidingNumber(x)))
            {
                return;
            }
            else
            {
                //throw new Exception("Not all elements are providing numbers");
            }
        }
    }
}
