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

        internal override void OverrideAnother(StoreDynamicDataStateProvider partiallyOverrided, out bool completelyOverrides)
        {
            throw new NotImplementedException();
        }
    }
}
