using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Dopple.InstructionNodes
{
    class LdElemAddressNode : InstructionNode, IArrayUsingNode, IIndexUsingNode
    {
        public LdElemAddressNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }

        public IEnumerable<InstructionNode> ArrayArgs
        {
            get
            {
                return DataFlowBackRelated.Where(x => x.ArgIndex == 0).Select(x => x.Argument);
            }
        }

        public IEnumerable<InstructionNode> IndexNodes
        {
            get
            {
                return DataFlowBackRelated.Where(x => x.ArgIndex == 1).Select(x => x.Argument);
            }
        }
    }
}
