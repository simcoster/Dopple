using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;

namespace DoppleTry2.InstructionModifiers
{
    class InitilizeStlocModifier : IModifier
    {
        public void Modify(List<InstructionWrapper> instructionWrappers)
        {
            var stLocCodes = new[] {Code.Stloc, Code.Stloc_0,Code.Stloc_1, Code.Stloc_2 , Code.Stloc_3, Code.Stloc_S};
            var instructions = instructionWrappers.Where(x => stLocCodes.Contains(x.Instruction.OpCode.Code));
            foreach (var instructionWrapper in instructions)
            {
                int location;
                switch (instructionWrapper.Instruction.OpCode.Code)
                {
                    case Code.Stloc_0:
                        location = 0;
                        break;
                    case Code.Stloc_1:
                        location = 1;
                        break;
                    case Code.Stloc_2:
                        location = 2;
                        break;
                    case Code.Stloc_3:
                        location = 3;
                        break;
                    case Code.Stloc_S: case Code.Stloc:
                        location = Convert.ToInt32(instructionWrapper.Instruction.Operand);
                        break;
                }
            }
        }
    }
}