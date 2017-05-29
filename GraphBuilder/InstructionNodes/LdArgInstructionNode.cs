using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Dopple.InstructionNodes
{
    [DataContract]
    public class LdArgInstructionNode : FunctionArgNodeBase
    {
        public LdArgInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }
        protected override bool TryGetCodeArgIndex(Instruction instruction, out int index)
        {
            switch (instruction.OpCode.Code)
            {
                case Code.Ldarg_0:
                    index = 0;
                    return true;
                case Code.Ldarg_1:
                    index = 1;
                    return true;
                case Code.Ldarg_2:
                    index = 2;
                    return true;
                case Code.Ldarg_3:
                    index = 3;
                    return true;
                default:
                    index = -1;
                    return false;
            }
        }
        public override int StackPopCount
        {
            get
            {
                if (InliningProperties.Inlined)
                {
                    return 0;
                }
                return base.StackPopCount;
            }

            protected set
            {
                base.StackPopCount = value;
            }
        }

        public override int DataFlowDataProdivderIndex
        {
            get
            {
                return 0;
            }
        }
    }


}