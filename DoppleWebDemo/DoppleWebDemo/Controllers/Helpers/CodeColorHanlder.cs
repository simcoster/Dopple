using Dopple;
using GraphSimilarityByMatching;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DoppleWebDemo.Controllers.Helpers
{
    class CodeColorHanlder
    {
        static Code[] LdcCodes = { Code.Ldc_I4_0, Code.Ldc_I4_1, Code.Ldc_I4_2, Code.Ldc_I4_3, Code.Ldc_I4_4, Code.Ldc_I4_5,
                            Code.Ldc_I4_6, Code.Ldc_I4_7, Code.Ldc_I4_8, Code.Ldc_I4_S, Code.Ldc_I4, Code.Ldc_R4,
                            Code.Ldc_R8, Code.Ldc_I8, Code.Ldc_I4_M1 };

        static Code[] LdargCodes = {Code.Ldarg, Code.Ldarg_0, Code.Ldarg_1, Code.Ldarg_2, Code.Ldarg_3, Code.Ldarg_S,
                             Code.Ldarga, Code.Ldarga_S };


        static Code[] LdStaticFld = { Code.Ldsfld, Code.Ldsflda };

        static Code[] LdElem = { Code.Ldelema,Code.Ldelem_I1,Code.Ldelem_U1,Code.Ldelem_I2
                         ,Code.Ldelem_U2 ,Code.Ldelem_I4 ,Code.Ldelem_U4 ,Code.Ldelem_I8
                         ,Code.Ldelem_I ,Code.Ldelem_R4 ,Code.Ldelem_R8 ,Code.Ldelem_Ref ,Code.Ldelem_Any};

        static Code[] LdFld = { Code.Ldfld, Code.Ldflda };

        static Code[] LdMemI = { Code.Ldind_I1, Code.Ldind_U1, Code.Ldind_I2, Code.Ldind_U2, Code.Ldind_I4, Code.Ldind_U4,
                          Code.Ldind_I8, Code.Ldind_I, Code.Ldind_R4, Code.Ldind_R8, Code.Ldind_Ref        };

        static Code[] StLoc = { Code.Stloc, Code.Stloc_0, Code.Stloc_1, Code.Stloc_2, Code.Stloc_3, Code.Stloc_S };

        static Code[] LdLoc = { Code.Ldloc_0, Code.Ldloc_1, Code.Ldloc_2, Code.Ldloc_3, Code.Ldloc, Code.Ldloc_S, Code.Ldloca_S, Code.Ldloca, };

        static Code[] Conv = { Code.Conv_I, Code.Conv_I1, Code.Conv_I2, Code.Conv_I4, Code.Conv_I8, Code.Conv_Ovf_I,
                        Code.Conv_Ovf_I_Un, Code.Conv_Ovf_I1, Code.Conv_Ovf_I1_Un, Code.Conv_Ovf_I2, Code.Conv_Ovf_I2_Un,
                        Code.Conv_Ovf_I4, Code.Conv_Ovf_I4_Un, Code.Conv_Ovf_I8, Code.Conv_Ovf_I8_Un, Code.Conv_Ovf_U,
                        Code.Conv_Ovf_U_Un, Code.Conv_Ovf_U1, Code.Conv_Ovf_U1_Un, Code.Conv_Ovf_U2, Code.Conv_Ovf_U2_Un,
                        Code.Conv_Ovf_U4, Code.Conv_Ovf_U4_Un, Code.Conv_Ovf_U8, Code.Conv_Ovf_U8_Un, Code.Conv_R_Un,
                        Code.Conv_R4, Code.Conv_R8, Code.Conv_U, Code.Conv_U1, Code.Conv_U2, Code.Conv_U4, Code.Conv_U8 };

        static Code[] Beq = { Code.Beq, Code.Beq_S };

        static Code[] Bge = { Code.Bge, Code.Bge_S, Code.Bge_Un, Code.Bge_Un_S };

        static Code[] Bgt = { Code.Bgt, Code.Bgt_S, Code.Bgt_Un, Code.Bgt_Un_S };

        static Code[] Ble = { Code.Ble, Code.Ble_S, Code.Ble_Un, Code.Ble_Un_S };

        static Code[] Blt = { Code.Blt, Code.Blt_S, Code.Blt_Un, Code.Blt_Un_S };

        Dictionary<Code, Color> CodeColors = new Dictionary<Code, Color>();

        public CodeColorHanlder()
        {
            int Rvalue = 50;
            int Gvalue = 50;
            int Bvalue = 50;

            int BigIncrement = 30;
            int SmallIncrement = 2;

            var deltCodes = GetType()
                .GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .Select(x => x.GetValue(null))
                .Cast<Code[]>();

            foreach (var codeGroup in deltCodes)
            {
                GetColor(ref Rvalue, ref Gvalue, ref Bvalue, BigIncrement);
                foreach (var code in codeGroup)
                {
                    CodeColors.Add(code, Color.FromArgb(Rvalue, Gvalue, Bvalue));
                    GetColor(ref Rvalue, ref Gvalue, ref Bvalue, SmallIncrement);
                }
            }

            var undeltCodes = CodeGroups.AllOpcodes.Select(x => x.Code).Except(deltCodes.SelectMany(x => x));

            foreach (var loneCode in undeltCodes)
            {
                GetColor(ref Rvalue, ref Gvalue, ref Bvalue, BigIncrement);
                CodeColors.Add(loneCode, Color.FromArgb(Rvalue, Gvalue, Bvalue));
            }
        }

        public Color GetNodeCodeColor(Code code)
        {

            return CodeColors[code];
        }

        public Color GetEdgeColor(int index, EdgeType edgeType)
        {
            if (edgeType == EdgeType.DataFlow)
            {
                switch (index)
                {
                    case 0:
                        return Color.Blue;
                    case 1:
                        return Color.Green;
                    case 2:
                        return Color.Yellow;
                    case 3:
                        return Color.Orange;
                    default:
                        return Color.Purple;
                }
            }
            if (edgeType == EdgeType.ProgramFlowAffecting)
            {
                switch (index)
                {
                    case 0:
                        return Color.LightPink;
                    case 1:
                        return Color.LightYellow;
                    case 2:
                        return Color.LightSeaGreen;
                    case 3:
                        return Color.LightBlue;
                    default:
                        return Color.LightGray;
                }
            }
            throw new Exception("invalid edge type");
        }

        private void GetColor(ref int RValue, ref int GValue, ref int BValue, int increment)
        {
            int maxValue = 255;

            RValue += increment;
            if (RValue > maxValue)
            {
                RValue -= maxValue;
                GValue += increment;
                if (GValue > maxValue)
                {
                    GValue -= maxValue;
                    BValue += increment;
                }
            }
        }
    }
}
