using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Runtime.Serialization;
using Dopple.InstructionNodeMembers;

namespace Dopple.InstructionNodes
{
    [DataContract]
    public class ConstructorCallNode : InlineableCallNode , IDataTransferingNode
    {
        public ConstructorCallNode(Instruction instruction, MethodDefinition targetMethod, MethodDefinition method, NewObjectNodeWithConstructor newObjectNode) : base(instruction, targetMethod, method)
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
}
