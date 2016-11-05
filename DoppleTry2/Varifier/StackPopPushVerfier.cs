using System;
using System.Collections.Generic;

namespace DoppleTry2.Varifier
{
    class StackPopPushVerfier : IVerifier
    {
        public void Verify(IEnumerable<InstructionWrapper> instructionWrappers)
        {
            foreach (var inst in instructionWrappers)
            {
                if ( inst.StackPushCount > 0 || inst.StackPopCount > 0)
                {
                    throw new Exception(String.Format("inst {0} has stack push count {1} and stack pop count {2}", inst.Instruction, inst.StackPushCount, inst.StackPopCount));
                }
            }
        }
    }
}