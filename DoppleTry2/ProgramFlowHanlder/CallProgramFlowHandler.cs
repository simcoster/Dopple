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
        readonly SimpleProgramFlowHandler _simpleProgramFlowHandler = new SimpleProgramFlowHandler();

        public override Code[] HandledCodes
        {
            get
            {
                return CodeGroups.CallCodes;
            }
        }

        protected override void SetForwardExecutionFlowInstsInternal(InstructionWrapper wrapperToModify, List<InstructionWrapper> instructionWrappers)
        {
            if (wrapperToModify.Inlined)
            {
                foreach (var inst in wrapperToModify.NextPossibleProgramFlow)
                {
                    inst.BackProgramFlow.Remove(wrapperToModify);
                }
                wrapperToModify.NextPossibleProgramFlow.Clear();
                TwoWayLinkExecutionPath(wrapperToModify, instructionWrappers[instructionWrappers.IndexOf(wrapperToModify) + 1]);
            }
            else
            {
                _simpleProgramFlowHandler.SetForwardExecutionFlowInsts(wrapperToModify, instructionWrappers);
            }
        }
    }
}
