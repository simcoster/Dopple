using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    public class ImmediateValueBackTracer : BackTracer
    {
        public Code[] LdcCodes =
        {
            Code.Ldc_I4_0, Code.Ldc_I4_1, Code.Ldc_I4_2, Code.Ldc_I4_3, Code.Ldc_I4_4, Code.Ldc_I4_5,
            Code.Ldc_I4_6, Code.Ldc_I4_7, Code.Ldc_I4_8, Code.Ldc_I4_S, Code.Ldc_I4, Code.Ldc_R4, Code.Ldc_R8,
            Code.Ldc_I8,
            Code.Ldc_I4_8
        };

        private Code[] LdArgCodes = 
        {
            Code.Ldarg_0 , Code.Ldarg_1 , Code.Ldarg_2 , Code.Ldarg_3 , Code.Ldarg_S , Code.Ldarga_S , Code.Ldarg , Code.Ldarga
        };

        private Code[] MoreCodes =
        {
            Code.Ldstr, Code.Ldnull, Code.Arglist
        };
        public ImmediateValueBackTracer(List<InstructionWrapper> instructionsWrappers, BackTraceManager manager) : base(instructionsWrappers)
        {
        }

        protected override IEnumerable<int> GetDataflowBackRelatedIndices(int instructionIndex)
        {
            throw new NotImplementedException();
        }

        public override Code[] HandlesCodes => 
            new []
            {
            Code.Ldc_I4_0, Code.Ldc_I4_1, Code.Ldc_I4_2, Code.Ldc_I4_3, Code.Ldc_I4_4, Code.Ldc_I4_5,
            Code.Ldc_I4_6, Code.Ldc_I4_7, Code.Ldc_I4_8,Code.Ldc_I4_S,Code.Ldc_I4,Code.Ldc_R4, Code.Ldc_R8,Code.Ldc_I8,
            Code.Ldc_I4_8 
            };
    }
}