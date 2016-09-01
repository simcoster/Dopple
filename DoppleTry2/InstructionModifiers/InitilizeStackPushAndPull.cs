using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace DoppleTry2.InstructionModifiers
{
    class InitilizeStackPushAndPull : IModifier
    {
        public void Modify(List<InstructionWrapper> instructionWrappers)
        {
            foreach (var instructionWrapper in instructionWrappers)
            {
                switch (instructionWrapper.Instruction.OpCode.StackBehaviourPush)
                {
                     case StackBehaviour.Push0:
                        instructionWrapper.StackPushCount = 0;
                        break;
                    case StackBehaviour.Push1_push1:
                        instructionWrapper.StackPushCount = 2;
                        break;
                    default:
                        instructionWrapper.StackPushCount = 1;
                        break;
                }
              
            }
        }
    }
}
