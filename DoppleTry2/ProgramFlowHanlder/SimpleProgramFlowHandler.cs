using System;
using System.Collections.Generic;
using System.Linq;
using Dopple.InstructionModifiers;
using Mono.Cecil.Cil;
using Dopple.InstructionNodes;

namespace Dopple.ProgramFlowHanlder
{
    class SimpleProgramFlowHandler : ProgramFlowHandler
    {
        public SimpleProgramFlowHandler()
        {
            HandledCodes = CodeGroups.AllOpcodes.Select(x => x.Code).ToArray();
        }

        //TODO check, why is this here?
        private readonly Code[] _unhandledCodes = new[] { Code.Br, Code.Br_S, Code.Ret };

        public override Code[] HandledCodes { get; }

        public override void SetForwardExecutionFlowInsts(InstructionNode node, List<InstructionNode> instructionWrappers)
        {
            var nodesPointingToMe =
               instructionWrappers.Where(x => x.Instruction.Next == node.Instruction);
           foreach(var nodePointingToMe in nodesPointingToMe)
            {
                node.ProgramFlowBackRoutes.AddTwoWay(nodesPointingToMe);
            }
        }
    }
}