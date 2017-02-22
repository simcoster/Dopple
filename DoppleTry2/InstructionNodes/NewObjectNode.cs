using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Dopple.InstructionNodes
{
    class NewObjectNode : InstructionNode
    {
        public int ConstructorParamCount { get; set; }
        public NewObjectNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            ConstructorParamCount = StackPopCount;
        }
    }
}
