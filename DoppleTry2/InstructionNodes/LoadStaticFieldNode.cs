using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Dopple.InstructionNodes
{
    internal class LoadStaticFieldNode : FieldManipulationNode
    {
        public LoadStaticFieldNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }
    }
}