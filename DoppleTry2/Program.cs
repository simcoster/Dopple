using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DoppleTry2.BackTrackers;
using Utility;

namespace DoppleTry2
{
    class Program
    {
        static readonly Code[] CallOpCodes = { Code.Call, Code.Calli, Code.Callvirt };
        public static readonly Code[] UnaffectingCodes = {Code.Nop, Code.Break};
        static void Main(string[] args)
        {
            //Creates an AssemblyDefinition from the "MyLibrary.dll" assembly
            AssemblyDefinition myLibrary = AssemblyDefinition.ReadAssembly(@"C:\Users\Simco\Documents\Visual Studio 2015\Projects\Dopple\Utility\bin\Debug\Utility.dll");

            TypeDefinition type = myLibrary.MainModule.Types[2];

            foreach (MethodDefinition func in type.Methods)
            {
                var inlinedInstructions  = DeepInline(func.Body.Instructions.ToList());
                inlinedInstructions = RemoveUnaffectingCodes(inlinedInstructions);
            }

        }

        static List<Instruction> DeepInline(List<Instruction> instructions)
        {
            for (int i =0;i< instructions.Count(); i++)
            {
                var inst = instructions.ElementAt(i);
                if (CallOpCodes.Any(x => x == inst.OpCode.Code && inst.Operand is MethodDefinition))
                {
                    var tempInst = inst;
                    instructions.Remove(inst);
                    var calledFuncInstructions = ((MethodDefinition)inst.Operand).Body.Instructions.Cast<Instruction>().ToList();
                    instructions.InsertRange( i, DeepInline(calledFuncInstructions));
                }
            }
            return instructions;
        }

        static List<Instruction> RemoveUnaffectingCodes(List<Instruction> instructions)
        {
            instructions.RemoveAll(x => UnaffectingCodes.Contains(x.OpCode.Code));
            return instructions;
        }
    }
}
