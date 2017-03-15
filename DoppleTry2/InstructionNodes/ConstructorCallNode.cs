using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Runtime.Serialization;

namespace Dopple.InstructionNodes
{
    [DataContract]
    public class ConstructorCallNode : InlineableCallNode , IDataTransferingNode
    {
        public ConstructorCallNode(Instruction instruction, MethodDefinition targetMethod, MethodDefinition method, ConstructorNewObjectNode newObjectNode) : base(instruction, targetMethod, method)
        {
            DataFlowBackRelated = new ConstructorCallDataFlowBackArgList(this);
            DataFlowBackRelated.AddTwoWay(newObjectNode, 0);
            StackPopCount--;
            StackPushCount = 1;
        }

        public int DataFlowDataProdivderIndex
        {
            get
            {
                return 0;
            }
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
