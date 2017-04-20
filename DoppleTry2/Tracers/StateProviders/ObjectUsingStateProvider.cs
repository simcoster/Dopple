using Dopple.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.Tracers.StateProviders
{
    abstract class ObjectUsingStateProvider : StoreDynamicDataStateProvider
    {
        public ObjectUsingStateProvider(InstructionNode storeNode) : base(storeNode)
        {
            ObjectNodes = GetObjectArgs();
            if (ObjectNodes.Count ==0)
            {
                throw new Exception("State provider must have object nodes");
            }
        }
        internal virtual List<InstructionNode> GetObjectArgs()
        {
            return StoreNode.DataFlowBackRelated.Where(x => x.ArgIndex == 0).SelectMany(x => x.Argument.GetDataOriginNodes()).ToList();
        }

        bool objectsContainVirtualNode = false;
        private List<InstructionNode> objectNodes;
        public List<InstructionNode> ObjectNodes
        {
            get
            {
                if (objectsContainVirtualNode || objectNodes.Count ==0)
                {
                    objectNodes = GetObjectArgs();
                }
                return objectNodes;
            }

            set
            {
                this.objectNodes = value;
                objectsContainVirtualNode = objectNodes.Any(x => x is VirtualCallInstructionNode);
            }
        }

        protected override void OverrideAnotherInternal(StoreDynamicDataStateProvider overrideCandidate, out bool completelyOverrides)
        {
            if (!IsRelatedToOtherStore(overrideCandidate))
            {
                completelyOverrides = false;
                return;
            }
            ObjectUsingStateProvider overrideCandidateAsObjectUsing = (ObjectUsingStateProvider) overrideCandidate;
            overrideCandidateAsObjectUsing.ObjectNodes = overrideCandidateAsObjectUsing.ObjectNodes.Except(ObjectNodes).ToList();
            completelyOverrides = overrideCandidateAsObjectUsing.objectNodes.Count == 0;
        }

        protected abstract bool IsRelatedToOtherStore(StoreDynamicDataStateProvider otherStateProvider);
    }
}
