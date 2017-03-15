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
    public class StArgInstructionNode : FunctionArgInstNode
    {
        public StArgInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method) { }

        public override int DataFlowDataProdivderIndex
        {
            get
            {
                return 0;
            }
        }
    }
}
