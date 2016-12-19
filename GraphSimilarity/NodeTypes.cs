using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarity
{
    public static class CodeTypes
    {
        public static Code[] Arithmatics = {Code.Add, Code.Add_Ovf, Code.Add_Ovf_Un, Code.Sub, Code.Sub_Ovf, Code.Sub_Ovf_Un,
                                            Code.Div, Code.Div_Un, Code.Mul, Code.Mul_Ovf, Code.Mul_Ovf_Un};
        public static Code[] CoditionalJumps = { Code.Beq, Code.Beq_S, Code.Bge, Code.Bge_S, Code.Bge_Un, Code.Bge_Un_S, Code.Bgt };
    }
}
