using Dopple.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.Tracers.PredciateProviders
{
    abstract class ObjectUsingStateProvider : StoreDynamicDataStateProvider
    {
        public ObjectUsingStateProvider(InstructionNode storeNode) : base(storeNode)
        {
            ObjectNodes = GetObjectArgs();
        }
        internal virtual List<InstructionNode> GetObjectArgs()
        {
            return StoreNode.DataFlowBackRelated.Where(x => x.ArgIndex == 0).SelectMany(x => x.Argument.GetDataOriginNodes()).ToList();
        }

        private List<InstructionNode> objectNodes;
        public List<InstructionNode> ObjectNodes
        {
            get
            {
                return objectNodes;
            }

            set
            {
                this.objectNodes = value;
            }
        }
    }
}
