using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DoppleTry2.InstructionNodes
{
    public class StIndInstructionNode : InstructionNode
    {
        public List<InstructionNode> AddressProvidingArgs = new List<InstructionNode>();
        public StIndInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }
    }
}
