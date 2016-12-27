using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionModifiers;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionNodes;

namespace DoppleTry2.ProgramFlowHanlder
{
    class MultiplePathsHandler : ProgramFlowHandler
    {
        public override Code[] HandledCodes => new[]
        {
            Code.Brfalse,Code.Brfalse_S,Code.Brtrue, Code.Brtrue_S, Code.Beq, Code.Beq_S, Code.Bge, Code.Bge_S, 
            Code.Bge_Un, Code.Bge_Un_S, Code.Bgt, Code.Bgt_S, Code.Bgt_Un, Code.Bgt_Un_S, Code.Ble, Code.Ble_S, 
            Code.Ble_Un, Code.Ble_Un_S, Code.Bne_Un, Code.Bne_Un_S, Code.Blt, Code.Blt_S, Code.Blt_Un, Code.Blt_Un_S, 
        };

        public override void SetForwardExecutionFlowInsts(InstructionNode wrapperToModify, List<InstructionNode> instructionWrappers)
        {
            var forwardInstruction = instructionWrappers.First(x => x.Instruction == wrapperToModify.Instruction.Operand);
            forwardInstruction.ProgramFlowBackRoutes.AddTwoWay(wrapperToModify);
        }
    }
}
