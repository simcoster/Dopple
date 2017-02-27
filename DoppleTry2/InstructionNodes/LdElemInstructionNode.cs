using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.InstructionNodes
{
    public class LdElemInstructionNode : DataTransferingNode
    {
        public LdElemInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            DataFlowBackRelated.MaxArgIndex = 2;
        }

        public override int DataFlowDataProdivderIndex
        {
            get
            {
                return 2;
            }
        }
    }
}
