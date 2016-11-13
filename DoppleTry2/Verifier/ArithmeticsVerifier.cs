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
                foreach(var arg in arithInst.BackDataFlowRelated.ArgumentList)
                {
                    var a = BackSearcher.GetStackPushAncestor(arg.Argument);
                    //if (BackSearcher.GetStackPushAncestor(arg.Argument).All(x => x is LdImmediateInstWrapper || (x is LdArgInstructionWrapper )))
                    //{

                    //}
                }
            }
        }

        public static bool IsNumber(object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }
    }
}
