using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.BranchPropertiesNS;
using Dopple.InstructionNodes;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Dopple.Tracers.PredciateProviders
{
    class StoreFieldStateProvider : ObjectUsingStateProvider
    {
        public StoreFieldStateProvider(InstructionNode storeNode) : base(storeNode)
        {
            _FieldDefinition = ((StoreFieldNode) storeNode).FieldDefinition;
        }

        private FieldDefinition _FieldDefinition { get; set; }

        public override bool IsLoadNodeMatching(InstructionNode loadNode)
        {
            LoadFieldNode loadFieldNode = loadNode as LoadFieldNode;
            if (loadFieldNode == null)
            {
                return false;
            }
            if (loadFieldNode.FieldDefinition.MetadataToken !=_FieldDefinition.MetadataToken)
            {
                return false;
            }
            var loadFieldObjectNodes = loadFieldNode.DataFlowBackRelated.Where(x => x.ArgIndex == 0).SelectMany(x => x.Argument.GetDataOriginNodes());
            if (!loadFieldObjectNodes.Intersect(ObjectNodes).Any()) 
            {
                return false;
            }
            return true;
        }

        private bool ShareNonObjectArgs(LoadFieldNode loadFieldNode)
        {
            if (this._FieldDefinition.MetadataToken != _FieldDefinition.MetadataToken)
            {
                return false;
            }
            return true;
        }

        internal override List<InstructionNode> GetObjectArgs()
        {
            return StoreNode.DataFlowBackRelated.Where(x => x.ArgIndex == 0).SelectMany(x => x.Argument.GetDataOriginNodes()).ToList();
        }

        protected override void OverrideAnotherInternal(StoreDynamicDataStateProvider overrideCandidate, out bool CompletelyOverrides)
        {
            var otherStoreFieldProvider = (StoreFieldStateProvider)overrideCandidate;
            if (otherStoreFieldProvider._FieldDefinition.MetadataToken != _FieldDefinition.MetadataToken)
            {
                CompletelyOverrides = false;
                return; 
            }
            otherStoreFieldProvider.ObjectNodes = otherStoreFieldProvider.ObjectNodes.Except(this.ObjectNodes).ToList();
            CompletelyOverrides = otherStoreFieldProvider.ObjectNodes.Count == 0;
        }
    }
}
