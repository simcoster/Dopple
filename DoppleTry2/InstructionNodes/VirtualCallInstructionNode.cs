using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.InstructionNodes
{
    internal class VirtualCallInstructionNode : NonInlineableCallInstructionNode
    {
        public bool ResolveAttempted { get; set; } = false;
        public List<InstructionNode> ResolvedObjectArgs = new List<InstructionNode>();
        internal VirtualCallInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }
    }
}
