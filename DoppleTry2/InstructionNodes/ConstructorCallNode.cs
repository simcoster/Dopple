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

        public ConstructorCallNode(Instruction instruction, MethodDefinition targetMethod, MethodDefinition method, ConstructorNewObjectNode newObjectNode) : base(instruction, targetMethod, method)
        {
            DataFlowBackRelated = new ConstructorCallDataFlowBackArgList(this);
            DataFlowBackRelated.AddTwoWay(newObjectNode, 0);
            StackPopCount--;
        }
    }

    public class ConstructorCallDataFlowBackArgList : DataFlowBackArgList
    {
        public ConstructorCallDataFlowBackArgList(InstructionNode instructionWrapper) : base(instructionWrapper)
        {
        }

        public override void AddTwoWay(InstructionNode toAdd)
        {
            var indexedToAdd = new IndexedArgument(CurrentIndex + 1, toAdd, this);
            AddTwoWay(indexedToAdd);
            CurrentIndex--;
        }
    }
}
