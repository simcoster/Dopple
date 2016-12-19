using DoppleTry2.InstructionWrappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoppleTry2.VerifierNs
{
    class StackPopPushVerfier : Verifier
    {
        public StackPopPushVerfier(List<InstructionWrapper> instructionWrappers) : base(instructionWrappers)
        {

        }

        public override void Verify(InstructionWrapper instructionWrapper)
        {
            if (instructionWrapper.Instruction.OpCode.Code == Mono.Cecil.Cil.Code.Ret && instructionWrapper.InliningProperties.Inlined == false)
            {
                return;
            }
            if (instructionWrapper.StackPushCount > 0 || instructionWrapper.StackPopCount > 0)
            {
                //throw new Exception(String.Format("inst {0} has stack push count {1} and stack pop count {2}", instructionWrapper.Instruction, instructionWrapper.StackPushCount, instructionWrapper.StackPopCount));
            }
        }
    }
}