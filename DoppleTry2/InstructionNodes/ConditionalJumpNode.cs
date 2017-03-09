using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Dopple.BackTracers;

namespace Dopple.InstructionNodes
{
    class ConditionalJumpNode : InstructionNode
    {
        public ConditionalJumpNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }
        public override void MergeInto(InstructionNode nodeToMergeInto, bool keepOriginal)
        {
            //TODO change
            //bool isRecursionMerge = nodeToMergeInto.Instruction == this.Instruction;
            //if (!isRecursionMerge)
            //{
            //    base.MergeInto(nodeToMergeInto, keepOriginal);
            //}
            //foreach (var affectedNodeInBoth in this.ProgramFlowForwardAffecting.Where(x => nodeToMergeInto.ProgramFlowForwardAffecting.Any(y => y.Argument == x.Argument)))
            //{
            //    affectedNodeInBoth.ArgIndex = (int)TrackType.Loop;
            //}
            base.MergeInto(nodeToMergeInto, keepOriginal);
        }
    }
}
