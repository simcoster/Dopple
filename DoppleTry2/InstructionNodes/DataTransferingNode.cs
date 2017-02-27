using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Dopple.InstructionNodes
{
    public abstract class DataTransferingNode : InstructionNode
    {
        public DataTransferingNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }
        public abstract int DataFlowDataProdivderIndex { get; }
    }
}
