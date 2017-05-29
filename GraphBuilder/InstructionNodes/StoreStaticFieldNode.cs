using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Dopple.InstructionNodes
{
    internal class StoreStaticFieldNode : FieldManipulationNode
    {
        public StoreStaticFieldNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }
    }
}