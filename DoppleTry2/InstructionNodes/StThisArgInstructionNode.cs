using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.InstructionNodes
{
    [DataContract]
    public class StThisArgInstructionNode : StoreArgumentNode
    {
        public StThisArgInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            ArgIndex = 0;
            ArgName = "this";
            ArgType = method.DeclaringType;
        }
    }
}
