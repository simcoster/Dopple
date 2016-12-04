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
    class AddHelperReturnInstsModifer : IModifier
    {
        public void Modify(List<InstructionWrapper> instructionWrappers)
        {
            foreach (var callInst in instructionWrappers.Where(x => CodeGroups.CallCodes.Contains(x.Instruction.OpCode.Code)).ToArray())
            {
                var opcode = Instruction.Create(OpCodes.Ret);
                InstructionWrapper retInstWrapper = InstructionWrapperFactory.GetInstructionWrapper(opcode, (MethodDefinition)callInst.Instruction.Operand);
                retInstWrapper.BackProgramFlow.AddTwoWay(callInst);
                foreach (var forwardFlowInst in callInst.ForwardProgramFlow)
                {
                    forwardFlowInst.BackProgramFlow.AddTwoWay(retInstWrapper);
                    forwardFlowInst.BackProgramFlow.RemoveAllTwoWay(x => x == callInst);
                }
                instructionWrappers.Insert(instructionWrappers.IndexOf(callInst) + 1, retInstWrapper);
            }
        }
    }
}
