using DoppleTry2.InstructionWrappers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.InstructionModifiers
{
    class AddHelperRetsModifer : IModifier
    {
        public void Modify(List<InstructionWrapper> instructionWrappers)
        {
            foreach (var callInst in instructionWrappers.Where(x => CodeGroups.CallCodes.Contains(x.Instruction.OpCode.Code)).ToArray())
            {
                var opcode = Instruction.Create(OpCodes.Ret);
                InstructionWrapper retInstWrapper = InstructionWrapperFactory.GetInstructionWrapper(opcode, (MethodDefinition)callInst.Instruction.Operand);
                retInstWrapper.BackProgramFlow.Add(callInst);
                retInstWrapper.ForwardProgramFlow.AddRange(callInst.ForwardProgramFlow);
                callInst.ForwardProgramFlow.Clear();
                callInst.ForwardProgramFlow.Add(retInstWrapper);
                instructionWrappers.Insert(instructionWrappers.IndexOf(callInst) + 1, retInstWrapper);
            }
        }
    }
}
