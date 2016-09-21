using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DoppleTry2.InstructionModifiers
{
    class InlineCall : IModifier
    {
        public static readonly Code[] CallOpCodes = { Code.Call, Code.Calli, Code.Callvirt };

        public void Modify(List<InstructionWrapper> instructionWrappers)
        {
            for (int i = 0; i < instructionWrappers.Count; i++)
            {
                var instWrapper = instructionWrappers[i];
                if (!(CallOpCodes.Contains(instWrapper.Instruction.OpCode.Code) &&
                            instWrapper.Instruction.Operand is MethodDefinition &&
                            instWrapper.Inlined == false))
                    return;
                var calledFuncInstructions = ((MethodDefinition)instWrapper.Instruction.Operand).Body.Instructions.ToList();
                calledFuncInstructions.RemoveAll(x => x.OpCode.Code == Code.Ret);
                var calledFunInstWrappers = calledFuncInstructions.Select(x => new InstructionWrapper(x)).ToList();
                instWrapper.Inlined = true;
                instructionWrappers.InsertRange(i, calledFunInstWrappers);
            }
        }
    }
}
