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
    public class LdElemInstructionNode : InstructionNode , IDataTransferingNode
    {
        public LdElemInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            DataFlowBackRelated.MaxArgIndex = 2;
        }

        public int DataFlowDataProdivderIndex
        {
            get
            {
                return 2;
            }
        }
    }
}
