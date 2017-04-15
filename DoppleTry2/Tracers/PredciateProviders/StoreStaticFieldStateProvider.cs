using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;
using Mono.Cecil;

namespace Dopple.Tracers.PredciateProviders
{
    class StoreStaticFieldStateProvider : StoreDynamicDataStateProvider
    {
        private FieldDefinition _FieldDefinition;

        public StoreStaticFieldStateProvider(InstructionNode storeNode) : base(storeNode)
        {
            _FieldDefinition = ((StoreFieldNode) storeNode).FieldDefinition;

        }

        public override bool IsLoadNodeMatching(InstructionNode loadNode)
        {
            var loadStaticFieldNode = loadNode as LoadStaticFieldNode;
            if (loadStaticFieldNode == null)
            {
                return false;
            }
            return loadStaticFieldNode.FieldDefinition.MetadataToken == _FieldDefinition.MetadataToken;
        }

        protected override void OverrideAnotherInternal(StoreDynamicDataStateProvider overrideCandidate, out bool completelyOverrides)
        {
            var otherStoreStaticField = (StoreStaticFieldStateProvider) overrideCandidate;
            completelyOverrides = this._FieldDefinition.MetadataToken == otherStoreStaticField._FieldDefinition.MetadataToken;
        }
    }
}
