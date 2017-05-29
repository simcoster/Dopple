using System.Linq;
using Mono.Cecil.Cil;

namespace Dopple.OpcodeProperties
{
    public class DataflowProperties
    {
        public static bool HasDataflowBackRelated(Code code)
        {
            return LdcCodes.Concat(LdArgCodes).Concat(MoreCodes).Contains(code);
        }

        public static Code[] LdcCodes =
            {
            Code.Ldc_I4_0, Code.Ldc_I4_1, Code.Ldc_I4_2, Code.Ldc_I4_3, Code.Ldc_I4_4, Code.Ldc_I4_5,
            Code.Ldc_I4_6, Code.Ldc_I4_7, Code.Ldc_I4_8, Code.Ldc_I4_S, Code.Ldc_I4, Code.Ldc_R4, Code.Ldc_R8,
            Code.Ldc_I8, Code.Ldc_I4_8
        };

        private static readonly Code[] LdArgCodes =
        {
            Code.Ldarg_0 , Code.Ldarg_1 , Code.Ldarg_2 , Code.Ldarg_3 , Code.Ldarg_S , Code.Ldarga_S , Code.Ldarg , Code.Ldarga
        };

        private static readonly Code[] MoreCodes =
        {
            Code.Ldstr, Code.Ldnull, Code.Arglist
        };
    }
}