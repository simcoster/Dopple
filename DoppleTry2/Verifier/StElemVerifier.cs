﻿using DoppleTry2.InstructionWrappers;
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
    class StElemVerifier : Verifier
    {
        public StElemVerifier(List<InstructionWrapper> instructionWrappers) : base(instructionWrappers)
        {
        }

        public override void Verify(InstructionWrapper instructionWrapper)
        {
            if (!CodeGroups.StElemCodes.Contains(instructionWrapper.Instruction.OpCode.Code))
            {
                return;
            }
            var ldArgGroup = instructionWrapper.BackDataFlowRelated.Where(x => x.ArgIndex == 2);
            if (!ldArgGroup.All(x => IsProvidingArray(x.Argument)))
            {
                throw new Exception("Bad array reference argument");
            }
            var locationArgGroup = instructionWrapper.BackDataFlowRelated.Where(x => x.ArgIndex == 1);
            if (!locationArgGroup.All(x => IsProvidingNumber(x.Argument)))
            {
                throw new Exception("Bad array location argument");
            }
            var valueArgGroup = instructionWrapper.BackDataFlowRelated.Where(x => x.ArgIndex == 0);
            if (!locationArgGroup.All(x => IsProvidingNumber(x.Argument)))
            {
                throw new Exception("Bad value argument");
            }
            if (instructionWrapper.BackDataFlowRelated.Max(x => x.ArgIndex) > 2)
            {
                throw new Exception("too many arguments!");
            }
        }
    }
}