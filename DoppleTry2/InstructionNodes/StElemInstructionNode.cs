using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using DoppleTry2.VerifierNs;

namespace DoppleTry2.InstructionNodes
{
    class StElemInstructionNode : InstructionNode
    {
        public StElemInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }
    }
}
