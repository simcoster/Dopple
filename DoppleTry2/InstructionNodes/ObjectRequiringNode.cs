using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Dopple.InstructionNodes
{
    public abstract class ObjectOrAddressRequiringNode : InstructionNode
    {
        public ObjectOrAddressRequiringNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }

        public virtual int ObjectOrAddressArgIndex
        {
            get
            {
                return 0;
            }
        }
        public bool ObjectOrAddressArgsResolved { get; set; }
    }
}