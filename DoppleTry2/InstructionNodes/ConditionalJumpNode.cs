using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Dopple.BackTracers;
using System.Runtime.Serialization;
using Dopple.BranchPropertiesNS;

namespace Dopple.InstructionNodes
{
    [DataContract]
    public class ConditionalJumpNode : InstructionNode
    {
        public List<BranchID> CreatedBranches = new List<BranchID>();

        protected override void TypeSpecificMerge(InstructionNode nodeToMergeInto)
        {
            var otherAsConditional = (ConditionalJumpNode) nodeToMergeInto;
            foreach(var branch in CreatedBranches)
            {
                otherAsConditional.CreatedBranches.Add(branch);
                branch.OriginatingNode = nodeToMergeInto;
            }
        }

        public ConditionalJumpNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }
    }

    public class PseudoSplitNode : ConditionalJumpNode
    {
        public PseudoSplitNode(MethodDefinition method) : base(Instruction.Create(CodeGroups.AllOpcodes.First(x => x.Code == Code.Nop)), method)
        {
            ProgramFlowResolveDone = true;
        }
    }
}
