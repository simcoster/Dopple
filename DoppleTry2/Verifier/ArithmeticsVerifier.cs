using DoppleTry2.InstructionWrappers;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.Verifier
{
    class ArithmeticsVerifier : IVerifier
    {
        public void Verify(IEnumerable<InstructionWrapper> instructionWrappers)
        {
            Code[] handledCodes = new[] {Code.Add,Code.Add_Ovf,Code.Add_Ovf_Un,
                   Code.Sub, Code.Sub_Ovf, Code.Sub_Ovf_Un,
                    Code.Div, Code.Div_Un,
                    Code.Mul, Code.Mul_Ovf, Code.Mul_Ovf_Un};

            foreach (var arithInst in instructionWrappers.Where(x => handledCodes.Contains(x.Instruction.OpCode.Code)).OrderByDescending(x => x.InstructionIndex))
            {
                foreach (var arg in arithInst.BackDataFlowRelated.ArgumentList)
                {
                    if (BackSearcher.GetStackPushAncestor(arg.Argument).All(x => x is LdImmediateInstWrapper || (x is LdArgInstructionWrapper && ((LdArgInstructionWrapper)x).ArgType.IsPrimitive)))
                    {

                    }
                    else
                    {

                    }
                }
            }
        }

        public static bool IsNumberType(Type value)
        {
            return value == typeof(sbyte)
                    || value == typeof(byte)
                    || value == typeof(short)
                    || value == typeof(ushort)
                    || value == typeof(int)
                    || value == typeof(uint)
                    || value == typeof(long)
                    || value == typeof(ulong)
                    || value == typeof(float)
                    || value == typeof(double)
                    || value == typeof(decimal);
        }
    }
}

