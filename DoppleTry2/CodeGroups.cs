using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2
{
    public static class CodeGroups
    {
        public static Code[] CallCodes     = { Code.Call, Code.Calli, Code.Callvirt };
        public static Code[] LdArgCodes    = { Code.Ldarg, Code.Ldarg_0, Code.Ldarg_1, Code.Ldarg_2, Code.Ldarg_3, Code.Ldarg_S,
                                               Code.Ldarga, Code.Ldarga_S};
        public static Code[] StArgCodes   =  { Code.Starg, Code.Starg_S};
        public static Code[] LdElemCodes  =  { Code.Ldelema, Code.Ldelem_Any, Code.Ldelem_I, Code.Ldelem_I1, Code.Ldelem_I2, Code.Ldelem_I4, Code.Ldelem_I8,
                                               Code.Ldelem_R4, Code.Ldelem_R8, Code.Ldelem_Ref, Code.Ldelem_U1 , Code.Ldelem_U2, Code.Ldelem_U4};
        public static Code[] StElemCodes =   {   Code.Stelem_Any, Code.Stelem_I, Code.Stelem_I2, Code.Stelem_I1, Code.Stelem_I4, Code.Stelem_I8, Code.Stelem_R4,
                                                Code.Stelem_R8, Code.Stelem_Ref };
        public static Code[] LdImmediateFromOperandCodes = { Code.Ldc_I4_S, Code.Ldc_I4, Code.Ldc_R4, Code.Ldc_R8, Code.Ldc_I8 };
        public static Code[] LdImmediateValueCodes = { Code.Ldc_I4_0, Code.Ldc_I4_1, Code.Ldc_I4_2, Code.Ldc_I4_3, Code.Ldc_I4_4, Code.Ldc_I4_5,
                                               Code.Ldc_I4_6, Code.Ldc_I4_7, Code.Ldc_I4_8};
        public static Code[] LdLocCodes   =  { Code.Ldloc_0, Code.Ldloc_1, Code.Ldloc_2, Code.Ldloc_3, Code.Ldloc, Code.Ldloc_S, Code.Ldloca_S, Code.Ldloca};
        public static Code[] StLocCodes   =  { Code.Stloc, Code.Stloc_0, Code.Stloc_1, Code.Stloc_2, Code.Stloc_3, Code.Stloc_S };
    }
}
