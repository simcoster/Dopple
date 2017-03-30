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
    internal class VirtualCallInstructionNode : NonInlineableCallInstructionNode
    {
        public bool ResolveAttempted { get; set; } = false;

        public int ObjectOrAddressArgIndex
        {
            get
            {
                return 0;
            }
        }

        public bool MethodIsUnresolveable {get;set;}

        public Dictionary<InstructionNode, bool> DataOriginNodeIsResolveable = new Dictionary<InstructionNode,bool>();
        internal VirtualCallInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }
    }
}
