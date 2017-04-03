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
    class StoreFieldStateProvider : StoreDynamicDataStateProvider
    {
        public StoreFieldStateProvider(InstructionNode storeNode) : base(storeNode)
        {
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
            if (loadNode.DataFlowBackRelated.Where(x => x.ArgIndex == 0).Select(x => x.Argument).Intersect(ObjectNodes).Any() == false)
            {
                return false;
            }
            return true;
        }

        public override bool ShareNonObjectArgs(StoreDynamicDataStateProvider newStore)
        {
            StoreFieldStateProvider newStateProvider = newStore as StoreFieldStateProvider;
            if (newStateProvider == null)
            {
                return false;
            }
            if (newStateProvider._FieldDefinition.MetadataToken != _FieldDefinition.MetadataToken)
            {
                return false;
            }
            return true;
        }

        internal override List<InstructionNode> GetObjectArgs(InstructionNode storeNode)
        {
            return StoreNode.DataFlowBackRelated.Where(x => x.ArgIndex == 0).SelectMany(x => x.Argument.GetDataOriginNodes()).ToList();
        }
    }
}
