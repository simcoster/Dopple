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
        public List<BranchID> CreatedBranches= new List<BranchID>();
        public List<InstructionNode> AffectedModes = new List<InstructionNode>();
        public Dictionary<InstructionNode, BranchID> ForwardBranchedPaths = new Dictionary<InstructionNode, BranchID>();
        public ConditionalJumpNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
        }
    }
}
