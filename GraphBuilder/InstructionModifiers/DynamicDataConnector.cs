using Dopple.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.InstructionModifiers
{
    class DynamicDataConnector
    {
        public static void DynamicStoreNodesRewire(List<InstructionNode> nodes)
        {
            foreach(var storeDynamicNode in nodes.Where(x => x is IDynamicDataStoreNode))
            {
                var loadDynamicNodeCast = (IDynamicDataStoreNode) storeDynamicNode;
                foreach(var fowrardDataNode in storeDynamicNode.DataFlowForwardRelated.ToList())
                {
                    var dataNodes = storeDynamicNode.DataFlowBackRelated.Where(x => x.ArgIndex == loadDynamicNodeCast.DataFlowDataProdivderIndex).ToList() ;
                    fowrardDataNode.MirrorArg.ContainingList.AddTwoWay(dataNodes.Select(x => x.Argument), fowrardDataNode.MirrorArg.ArgIndex);
                    storeDynamicNode.DataFlowForwardRelated.RemoveTwoWay(fowrardDataNode);
                }
            }
        }

        public static void DynamicLoadNodesRewire(List<InstructionNode> nodes)
        {
            foreach (var loadDynamicNode in nodes.Where(x => x is IDynamicDataLoadNode).ToList())
            {
                var loadDynamicNodeCast = (IDynamicDataLoadNode) loadDynamicNode;
                foreach (var fowrardDataNode in loadDynamicNode.DataFlowForwardRelated.ToList())
                {
                    var storeNodes = loadDynamicNode.DataFlowBackRelated.Where(x => x.ArgIndex == loadDynamicNodeCast.DataFlowDataProdivderIndex).ToList();
                    fowrardDataNode.MirrorArg.ContainingList.AddTwoWay(storeNodes.Select(x => x.Argument), fowrardDataNode.MirrorArg.ArgIndex);
                }
                if (loadDynamicNodeCast.AllPathsHaveAStoreNode)
                {
                    loadDynamicNode.SelfRemove();
                    nodes.Remove(loadDynamicNode);
                }
            }
        }
    }
}
