using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;
using Mono.Cecil.Cil;

namespace Dopple.BackTracers
{
    /// <summary>
    /// if all paths deam this load command as connected to a store command, then it is only passing data through
    /// otherwise (if some paths do not) then we leave the "dirty" pseudo node as well
    /// </summary>
    abstract class DynamicDataBacktracer : DataflowBacktracer
    {
        protected override IEnumerable<InstructionNode> GetDataflowBackRelatedArgGroup(InstructionNode instructionNode)
        {
            bool allPathsHaveAMatch;
            var foundInstructions = SingleIndexBackSearcher.SafeSearchBackwardsForDataflowInstrcutions(GetPredicate(instructionNode), instructionNode, out allPathsHaveAMatch);
            if (allPathsHaveAMatch)
            {
                instructionNode.DataFlowBackRelated.DynamicDataFullyTraced = true;
            }
            return foundInstructions;           
        }

        protected abstract Predicate<InstructionNode> GetPredicate(InstructionNode instructionNode);
    }
}
