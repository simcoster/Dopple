using Dopple.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.InstructionNodeMembers
{
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
