using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionModifiers;
using Mono.Cecil.Cil;

namespace DoppleTry2.ProgramFlowHanlder
{
    class MultiplePathsHandler : ForwardPathHanlder
    {
        public MultiplePathsHandler(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override Code[] HandledCodes => new[]
        {
            Code.Brfalse,Code.Brfalse_S,Code.Brtrue, Code.Brtrue_S, Code.Beq, Code.Beq_S, Code.Bge, Code.Bge_S, 
            Code.Bge_Un, Code.Bge_Un_S, Code.Bgt, Code.Bgt_S, Code.Bgt_Un, Code.Bgt_Un_S, Code.Ble, Code.Ble_S, 
            Code.Ble_Un, Code.Ble_Un_S, br 
        };
        protected override void SetForwardExecutionFlowInsts(InstructionWrapper instructionWrapper)
        {
            int instIndex = InstructionsWrappers.IndexOf(instructionWrapper);
            TwoWayLinkExecutionPath(instructionWrapper,InstructionsWrappers[instIndex +1]);
            var secondForwardInstruction = InstructionsWrappers.First(x => x.Instruction ==instructionWrapper.Instruction.Operand);
            TwoWayLinkExecutionPath(instructionWrapper, secondForwardInstruction);
        }
    }
}
