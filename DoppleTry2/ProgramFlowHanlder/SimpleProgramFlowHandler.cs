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
            HandledCodes = CodeGroups.AllOpcodes.Select(x => x.Code).Except(_unhandledCodes).ToArray();
        }

        //TODO check, why is this here?
        private readonly Code[] _unhandledCodes = new[] { Code.Br, Code.Br_S, Code.Ret };

        public override Code[] HandledCodes { get; }

        public override void SetForwardExecutionFlowInsts(InstructionNode node, List<InstructionNode> instructionWrappers)
        {
            var pointedAtNode =
               instructionWrappers.FirstOrDefault(x => x.Instruction == node.Instruction.Next);
            if (pointedAtNode ==null)
            {
                Console.WriteLine("node "+ node.Instruction +" is pointing to no one, this shouldn't happen, investigate");
                return;
            }
            pointedAtNode.ProgramFlowBackRoutes.AddTwoWay(node);
        }
    }
}