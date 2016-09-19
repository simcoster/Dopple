using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace DoppleTry2
{
    class LdStLocProperties
    {
        private static readonly Dictionary<Code, int> LcCodesIndexes = new Dictionary<Code, int>
        {
            {Code.Ldloc_0, 0},
            {Code.Ldloc_1, 1},
            {Code.Ldloc_2, 2},
            {Code.Ldloc_3, 3},
            {Code.Stloc_0, 0},
            {Code.Stloc_1, 1},
            {Code.Stloc_2, 2},
            {Code.Stloc_3, 3},

        };

        private static readonly List<Code> LcCodesOperandIndex = new List<Code>
        {
            Code.Ldloc,
            Code.Ldloca,
            Code.Ldloc_S,
            Code.Ldloca_S,
            Code.Stloc,
            Code.Stloc_S

        };

        public static int GetLocIndex(Instruction instruction)
        {
            if (LcCodesIndexes.ContainsKey(instruction.OpCode.Code))
            {
                return LcCodesIndexes[instruction.OpCode.Code];
            }
            if (LcCodesOperandIndex.Contains(instruction.OpCode.Code))
            {
                return (int)instruction.Operand;
            }
            else
            {
                return -1;
            }
        }
    }
}
