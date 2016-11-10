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
        public static Code[] CallCodes =     { Code.Call, Code.Calli, Code.Callvirt };
        public static Code[] LocStoreCodes = { Code.Stloc, Code.Stloc_0, Code.Stloc_1, Code.Stloc_2, Code.Stloc_3, Code.Stloc_S };
        public static Code[] LdArgCodes =    { Code.Ldarg, Code.Ldarg_0, Code.Ldarg_1, Code.Ldarg_2, Code.Ldarg_3, Code.Ldarg_S,
                                               Code.Ldarga, Code.Ldarga_S};
        public static Code[] StArgCodes =    { Code.Starg, Code.Starg_S};
        public static Code[] LocLoadCodes =  { Code.Ldloc_0, Code.Ldloc_1, Code.Ldloc_2, Code.Ldloc_3, Code.Ldloc, Code.Ldloc_S, Code.Ldloca_S, Code.Ldloca, };
        public static Code[] LdElemCodes =   { Code.Ldelema, Code.Ldelem_Any, Code.Ldelem_I, Code.Ldelem_I1, Code.Ldelem_I2, Code.Ldelem_I4, Code.Ldelem_I8,
                                               Code.Ldelem_R4, Code.Ldelem_R8, Code.Ldelem_Ref, Code.Ldelem_U1 , Code.Ldelem_U2, Code.Ldelem_U4};
    }
}
