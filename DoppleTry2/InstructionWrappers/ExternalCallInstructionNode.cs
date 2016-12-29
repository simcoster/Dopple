using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DoppleTry2.InstructionNodes
{
    internal class ExternalCallInstructionNode : InstructionNode
    {
        public ExternalCallInstructionNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            var targetMethod = (MethodReference) instruction.Operand;
            StackPopCount = targetMethod.Parameters.Count;
            if (targetMethod.FullName == "System.Void")
            {
                StackPushCount = 0;
            }
            else
            {
                StackPushCount = 1;
            }
        }
    }
}