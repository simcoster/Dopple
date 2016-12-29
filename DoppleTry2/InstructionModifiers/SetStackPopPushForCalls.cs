using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionNodes;

namespace DoppleTry2.InstructionModifiers
{
    class SetStackPopPushForCalls : IModifier
    {
        public void Modify(List<InstructionNode> instructionNodes)
        {
            foreach(var callNode in instructionNodes.Where(x => x is InternalCallInstructionNode))
            {

            }
        }
    }
}
