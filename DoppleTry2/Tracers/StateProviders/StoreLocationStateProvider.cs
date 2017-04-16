using Dopple.Tracers.StateProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;
using Mono.Cecil.Cil;

namespace Dopple.Tracers.StateProviders
{
    class StoreLocationStateProvider : StoreDynamicDataStateProvider
    {
        LocationStoreInstructionNode LocStoreNode;
        public StoreLocationStateProvider(InstructionNode storeNode) : base(storeNode)
        {
            LocStoreNode = (LocationStoreInstructionNode) storeNode;
        }

        Code[] LoadLocationCodes = CodeGroups.LdLocCodes.Concat(new[] { Code.Ldloca, Code.Ldloca_S }).ToArray();

        public override bool IsLoadNodeMatching(InstructionNode loadNode)
        {
            if (!LoadLocationCodes.Contains(loadNode.Instruction.OpCode.Code))
            {
                return false;
            }
            return ((LocationLoadInstructionNode) loadNode).LocIndex == LocStoreNode.LocIndex && loadNode.Method == StoreNode.Method;
        }

        protected override void OverrideAnotherInternal(StoreDynamicDataStateProvider overrideCandidate, out bool completelyOverrides)
        {
            completelyOverrides = ((StoreLocationStateProvider) overrideCandidate).LocStoreNode.LocIndex == LocStoreNode.LocIndex &&
                                   overrideCandidate.StoreNode.Method == StoreNode.Method;
        }
    }
}
