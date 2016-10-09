using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace DoppleTry2.ProgramFlowHanlder
{
    class CallProgramFlowHandler : ProgramFlowHandler
    {

        SimpleProgramFlowHandler SimpleProgramFlowHandler; 
        public CallProgramFlowHandler(List<InstructionWrapper> instructionWrappers) : base(instructionWrappers)
        {
            SimpleProgramFlowHandler=  new SimpleProgramFlowHandler(InstructionWrappers);
        }

        public override Code[] HandledCodes
        {
            get
            {
                return new[] { Code.Call, Code.Calli, Code.Callvirt };
            }
        }

        public override void SetForwardExecutionFlowInsts(InstructionWrapper instructionWrapper)
        {
            if (instructionWrapper.Inlined)
            {
                foreach (var inst in instructionWrapper.NextPossibleProgramFlow)
                {
                    inst.BackProgramFlow.Remove(instructionWrapper);
                }
                instructionWrapper.NextPossibleProgramFlow.Clear();
                TwoWayLinkExecutionPath(instructionWrapper, InstructionWrappers[InstructionWrappers.IndexOf(instructionWrapper) + 1]); 
            }
            else
            {
                SimpleProgramFlowHandler.SetForwardExecutionFlowInsts(instructionWrapper);
            }
        }
    }
}
