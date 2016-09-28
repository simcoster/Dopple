using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    public class ImmediateValueBackTracer : BackTracer
    {
        private readonly Code[] _ldcCodes =
        {
            Code.Ldc_I4_0, Code.Ldc_I4_1, Code.Ldc_I4_2, Code.Ldc_I4_3, Code.Ldc_I4_4, Code.Ldc_I4_5,
            Code.Ldc_I4_6, Code.Ldc_I4_7, Code.Ldc_I4_8, Code.Ldc_I4_S, Code.Ldc_I4, Code.Ldc_R4, Code.Ldc_R8,
            Code.Ldc_I8, Code.Ldc_I4_8, Code.Ldc_I4_M1
        };

        private readonly Code[] _moreCodes =
        {
            Code.Ldstr, Code.Ldnull, Code.Arglist
        };
        public ImmediateValueBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override IEnumerable<InstructionWrapper> GetDataflowBackRelatedIndices(InstructionWrapper instWrapper)
        {
            return new InstructionWrapper[0];
        }

        public override Code[] HandlesCodes =>
            _ldcCodes.Concat(_moreCodes).ToArray();
    }
}