using System;
using Dopple.InstructionNodes;
using Mono.Cecil.Cil;

namespace Dopple.BackTracers
{
    internal class DynaymicLdArgBacktracer : DynamicDataBacktracer
    {
        public override Code[] HandlesCodes
        {
            get
            {
                return CodeGroups.LdArgCodes;
            }
        }

        protected override Predicate<InstructionNode> GetPredicate(InstructionNode instructionNode)
        {
            Predicate<InstructionNode> predicate = (x => x is StoreArgumentNode && ((StoreArgumentNode) x).ArgIndex == ((LdArgInstructionNode) instructionNode).ArgIndex);
            return predicate;
        }
    }
}