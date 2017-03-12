﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using Dopple.InstructionNodes;

namespace Dopple.BackTracers
{
    class LoadMemoryByOperandBackTracer : SingeIndexBackTracer
    {
        protected override IEnumerable<InstructionNode> GetDataflowBackRelatedArgGroup(InstructionNode instWrapper)
        {
            var storeIndex =
            SingleIndexBackSearcher.SafeSearchBackwardsForDataflowInstrcutions(x => x.MemoryStoreCount > 0 &&
                                                        instWrapper.Instruction.Operand ==
                                                        x.Instruction.Operand, instWrapper);
            return storeIndex;
        }

        public override Code[] HandlesCodes => new[]
        {
            Code.Ldind_I1, Code.Ldind_U1, Code.Ldind_I2,
            Code.Ldind_U2, Code.Ldind_I4, Code.Ldind_U4,
            Code.Ldind_I8, Code.Ldind_I, Code.Ldind_R4,
            Code.Ldind_R8, Code.Ldind_Ref};

    }
}
