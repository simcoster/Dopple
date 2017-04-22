using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple
{
    public static class CodeGroups
    {
        public static OpCode[] AllOpcodes = typeof(OpCodes).GetFields().Select(x => x.GetValue(null)).Cast<OpCode>().ToArray();
        public static Code[] CallCodes     = { Code.Call, Code.Calli};
        public static Code[] LdArgCodes    = { Code.Ldarg, Code.Ldarg_0, Code.Ldarg_1, Code.Ldarg_2, Code.Ldarg_3, Code.Ldarg_S};
        public static Code[] StArgCodes   =  { Code.Starg, Code.Starg_S};
        public static Code[] LdElemCodes  =  { Code.Ldelem_Any, Code.Ldelem_I, Code.Ldelem_I1, Code.Ldelem_I2, Code.Ldelem_I4, Code.Ldelem_I8,
                                               Code.Ldelem_R4, Code.Ldelem_R8, Code.Ldelem_Ref, Code.Ldelem_U1 , Code.Ldelem_U2, Code.Ldelem_U4};
        public static Code[] StElemCodes =   {   Code.Stelem_Any, Code.Stelem_I, Code.Stelem_I2, Code.Stelem_I1, Code.Stelem_I4, Code.Stelem_I8, Code.Stelem_R4,
                                                Code.Stelem_R8, Code.Stelem_Ref };
        public static Code[] LdImmediateFromOperandCodes = { Code.Ldc_I4_S, Code.Ldc_I4, Code.Ldc_R4, Code.Ldc_R8, Code.Ldc_I8 };
        public static Code[] LdImmediateValueCodes = { Code.Ldc_I4_0, Code.Ldc_I4_1, Code.Ldc_I4_2, Code.Ldc_I4_3, Code.Ldc_I4_4, Code.Ldc_I4_5,
                                               Code.Ldc_I4_6, Code.Ldc_I4_7, Code.Ldc_I4_8, Code.Ldc_I4_M1};
        public static Code[] LdLocCodes   =  { Code.Ldloc_0, Code.Ldloc_1, Code.Ldloc_2, Code.Ldloc_3, Code.Ldloc, Code.Ldloc_S};
        public static Code[] LdLocAddressCodes = { Code.Ldloca, Code.Ldloca_S};
        public static Code[] StLocCodes   =  { Code.Stloc, Code.Stloc_0, Code.Stloc_1, Code.Stloc_2, Code.Stloc_3, Code.Stloc_S };
        public static Code[] CondJumpCodes = { Code.Beq, Code.Beq_S, Code.Bge_S, Code.Bge_Un, Code.Bge_Un_S, Code.Bgt, Code.Bgt_S, Code.Bgt_Un, Code.Bgt_Un_S,
                                               Code.Ble, Code.Ble_S, Code.Ble_Un, Code.Ble_Un_S, Code.Blt, Code.Blt_S, Code.Blt_Un, Code.Blt_Un_S, Code.Bne_Un,
                                               Code.Bne_Un_S, Code.Brfalse , Code.Brfalse_S, Code.Brtrue, Code.Brtrue_S, Code.Switch };
        public static Code[] ArithmeticCodes = {Code.Add, Code.Add_Ovf, Code.Add_Ovf_Un, Code.Sub, Code.Sub_Ovf, Code.Sub_Ovf_Un, Code.Mul, Code.Mul_Ovf, Code.Mul_Ovf_Un,
                                                Code.Div, Code.Div_Un};
        public static Code[] StIndCodes = { Code.Stind_I, Code.Stind_I1, Code.Stind_I2, Code.Stind_I4, Code.Stind_I8, Code.Stind_R4, Code.Stind_R8 };
        public static Code[] LoadFieldCodes = { Code.Ldfld};
        public static Code[] StoreFieldCodes = { Code.Stfld, Code.Stsfld};
        public static Code[] LdIndCodes = { Code.Ldind_I, Code.Ldind_I1, Code.Ldind_I2, Code.Ldind_I4, Code.Ldind_I8, Code.Ldind_R4 , Code.Ldind_R8, Code.Ldind_Ref, Code.Ldind_U1, Code.Ldind_U2, Code.Ldind_U4};
        public static Code[][] CodeGroupLists = typeof(CodeGroups).GetFields().Select(x => x.GetValue(null)).Where(x => x is Code[]).Cast<Code[]>().ToArray();
        public static bool AreSameGroup (Code firstCode, Code secondCode)
        {
            if (firstCode == secondCode)
            {
                return true;
            }
            return CodeGroupLists.Any(x => x.Contains(firstCode) && x.Contains(secondCode));
        }
    }
}
