using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionWrappers;


namespace DoppleTry2.ProgramFlowHanlder
{
    class CallProgramFlowHandler : ProgramFlowHandler
    {
        readonly SimpleProgramFlowHandler _simpleProgramFlowHandler = new SimpleProgramFlowHandler();

        public override Code[] HandledCodes
        {
            get
            {
                return CodeGroups.CallCodes;
            }
        }

        public override void SetForwardExecutionFlowInsts(InstructionWrapper wrapperToModify, List<InstructionWrapper> instructionWrappers)
        {
            if (wrapperToModify.InliningProperties.Inlined)
            {
                foreach (var inst in wrapperToModify.ForwardProgramFlow)
                {
                    inst.BackProgramFlow.RemoveTwoWay(wrapperToModify);
                }
                instructionWrappers[instructionWrappers.IndexOf(wrapperToModify) + 1].BackProgramFlow.AddTwoWay(wrapperToModify);
            }
            else
            {
                _simpleProgramFlowHandler.SetForwardExecutionFlowInsts(wrapperToModify, instructionWrappers);
            }
        }
    }
}
