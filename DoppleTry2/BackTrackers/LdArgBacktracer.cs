using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionNodes;

namespace DoppleTry2.BackTrackers
{
    class LdArgBacktracer : SingeIndexBackTracer
    {
        public LdArgBacktracer(List<InstructionNode> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override IEnumerable<InstructionNode> GetDataflowBackRelatedArgGroup(InstructionNode instWrapper)
        {
            List<List<InstructionNode>> backRelated = new List<List<InstructionNode>>();
            List<InstructionNode> stArgInst = new List<InstructionNode>();
            if (instWrapper.InliningProperties.Inlined)
            {
                return _SingleIndexBackSearcher.SearchBackwardsForDataflowInstrcutions(x => x is StArgInstructionWrapper &&
                                                                    ((StArgInstructionWrapper)x).ArgIndex == ((LdArgInstructionNode)instWrapper).ArgIndex, instWrapper);
            }
            else
            {
                return _SingleIndexBackSearcher.SafeSearchBackwardsForDataflowInstrcutions(x => x is StArgInstructionWrapper && ((StArgInstructionWrapper)x).ArgIndex == ((LdArgInstructionNode)instWrapper).ArgIndex, instWrapper);
            }
        }

        public override Code[] HandlesCodes => CodeGroups.LdArgCodes; 
    }
}
