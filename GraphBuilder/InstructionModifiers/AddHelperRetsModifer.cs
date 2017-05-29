using Dopple.InstructionNodes;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.InstructionModifiers
{
    class AddHelperReturnInstsModifer : IModifier
    {
        private InstructionNodeFactory _InstructionNodeFactory = new InstructionNodeFactory();
        public void Modify(List<InstructionNode> instructionWrappers)
        {
            foreach (var callInst in instructionWrappers.Where(x => CodeGroups.CallCodes.Contains(x.Instruction.OpCode.Code)).ToArray())
            {
                var opcode = Instruction.Create(OpCodes.Ret);
                InstructionNode retInstWrapper = _InstructionNodeFactory.GetSingleInstructionNode(opcode, (MethodDefinition)callInst.Instruction.Operand);
                retInstWrapper.ProgramFlowBackRoutes.AddTwoWay(callInst);
                foreach (var forwardFlowInst in callInst.ProgramFlowForwardRoutes)
                {
                    forwardFlowInst.ProgramFlowBackRoutes.AddTwoWay(retInstWrapper);
                    forwardFlowInst.ProgramFlowBackRoutes.RemoveAllTwoWay(x => x == callInst);
                }
                instructionWrappers.Insert(instructionWrappers.IndexOf(callInst) + 1, retInstWrapper);
            }
        }
    }
}
