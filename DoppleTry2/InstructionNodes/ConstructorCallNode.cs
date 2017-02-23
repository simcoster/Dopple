using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Dopple.InstructionNodes
{
    public class ConstructorCallNode : InlineableCallNode
    {
        public ConstructorCallNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {

        }
        public ConstructorCallNode(Instruction instruction, MethodDefinition targetMethod, MethodDefinition method) : base(instruction,targetMethod,method)
        {

        }
    }
}
