using Dopple.InstructionNodes;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.Tracers.StateProviders
{
    class StoreArgumentStateProvider : StoreDynamicDataStateProvider
    {
        FunctionArgNodeBase ArgumentNode;
        public StoreArgumentStateProvider(InstructionNode storeNode) : base(storeNode)
        {
            ArgumentNode = (FunctionArgNodeBase) storeNode;
        }

        Code[] LoadLocationCodes = CodeGroups.LdArgCodes.Concat(new[] { Code.Ldarga, Code.Ldarga_S }).ToArray();

        public override bool IsLoadNodeMatching(InstructionNode loadNode)
        {
            if (!LoadLocationCodes.Contains(loadNode.Instruction.OpCode.Code))
            {
                return false;
            }
            return ((LdArgInstructionNode) loadNode).ArgIndex == ArgumentNode.ArgIndex && loadNode.Method == StoreNode.Method;
        }

        protected override void OverrideAnotherInternal(StoreDynamicDataStateProvider overrideCandidate, out bool completelyOverrides)
        {
            completelyOverrides = ((StoreArgumentStateProvider) overrideCandidate).ArgumentNode.ArgIndex == ArgumentNode.ArgIndex &&
                                   overrideCandidate.StoreNode.Method == StoreNode.Method;
        }
    }
}
