using Dopple.BackTracers;
using Dopple.InstructionNodes;
using Dopple.Tracers.PredciateProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.Tracers.DynamicTracing
{
    public class DynamicDataTracer
    {
        private int CountVisitedNodes;
        private List<InstructionNode> GlobalVisited = new List<InstructionNode>();

        internal void BackTraceOutsideFunctionBounds(List<InstructionNode> instructionNodes)
        {
            CountVisitedNodes = 0;
            var mergingNodesData = new Dictionary<InstructionNode, MergeNodeTraceData>();
            foreach (var mergingNode in instructionNodes.Where(x => x.ProgramFlowBackRoutes.Count > 1))
            {
                mergingNodesData.Add(mergingNode, new MergeNodeTraceData());
            }
            TraceOutsideFunctionBoundsRec(instructionNodes[0], mergingNodesData);
            var nonPassedThrough = instructionNodes.Except(GlobalVisited).ToArray().OrderBy(x => x.InstructionIndex).ToArray();
            if (nonPassedThrough.Any())
            {
                var wasntReached = instructionNodes.Except(GlobalVisited).ToArray().OrderBy(x => x.InstructionIndex).First();
                Console.WriteLine("Node wasn't reached is " + wasntReached.InstructionIndex + " " + wasntReached.Instruction);
                //throw new Exception("some nodes not reached");
            }
            Console.WriteLine("Visited node count is " + GlobalVisited.Count);
            CountVisitedNodes = 0;
            ForwardDynamicData(instructionNodes);
        }

        private void ForwardDynamicData(List<InstructionNode> instructionNodes)
        {
            foreach(var node in instructionNodes.Where(x => x is IDynamicDataLoadNode))
            {
                var loadedDynamicData = node.DataFlowBackRelated.Where(x => x.ArgIndex == ((IDynamicDataLoadNode) node).DataFlowDataProdivderIndex).Select(x => x.Argument).ToList();
                foreach (var forwardNode in node.DataFlowForwardRelated)
                {
                    forwardNode.Argument.DataFlowBackRelated.AddTwoWay(loadedDynamicData, forwardNode.ArgIndex);
                }
                node.DataFlowBackRelated.RemoveAll(x => loadedDynamicData.Contains(x.Argument));
            }
        }

        private void TraceOutsideFunctionBoundsRec(InstructionNode currentNode, Dictionary<InstructionNode, MergeNodeTraceData> mergingNodesData, InstructionNode lastNode = null, StateProviderCollection stateProviders = null, List<InstructionNode> visited = null)
        {
            if (visited == null)
            {
                visited = new List<InstructionNode>();
                lastNode = currentNode;
                stateProviders = new StateProviderCollection();
                GlobalVisited.Clear();
            }
            while (true)
            {
                if (visited.Contains(currentNode))
                {
                    if (!currentNode.BranchProperties.MergingNodeProperties.IsMergingNode)
                    {
                        return;
                    }
                }
                visited.Add(currentNode);
                GlobalVisited.Add(currentNode);
                bool reachedMergeNodeNotLast;
                int stateProviderCount = stateProviders.Count;
                ActOnCurrentNode(currentNode, mergingNodesData, lastNode, stateProviders, out reachedMergeNodeNotLast);
                if (reachedMergeNodeNotLast)
                {
                    return;
                }
                lastNode = currentNode;
                if (currentNode.ProgramFlowForwardRoutes.Count == 1)
                {
                    currentNode = currentNode.ProgramFlowForwardRoutes[0];
                }
                else
                {
                    break;
                }
            }
            if (currentNode.ProgramFlowForwardRoutes.Count == 0)
            {
                return;
            }
            if (!(currentNode is ConditionalJumpNode))
            {
                Console.WriteLine("Splitting without a conditional");
            }
            foreach (var node in currentNode.ProgramFlowForwardRoutes)
            {
                TraceOutsideFunctionBoundsRec(node, mergingNodesData, currentNode, stateProviders, visited);
            }
        }

        private static void ActOnCurrentNode(InstructionNode currentNode, Dictionary<InstructionNode, MergeNodeTraceData> mergingNodesData, InstructionNode lastNode, StateProviderCollection stateProviders, out bool reachedMergeNodeNotLast)
        {
            if (currentNode.BranchProperties.MergingNodeProperties.IsMergingNode)
            {
                lock (mergingNodesData)
                {
                    mergingNodesData[currentNode].ReachedNodes.Add(lastNode);
                    mergingNodesData[currentNode].AccumelatedStateProviders.AddNewProviders(stateProviders);
                    bool allBranchesReached = !currentNode.ProgramFlowBackRoutes.Except(mergingNodesData[currentNode].ReachedNodes).Any();
                    if (allBranchesReached)
                    {
                        mergingNodesData[currentNode].AllBranchesReached = true;
                        mergingNodesData[currentNode].AccumelatedStateProviders.MergeBranches(currentNode);
                        stateProviders.AddNewProviders(mergingNodesData[currentNode].AccumelatedStateProviders);
                    }
                    else
                    {
                        reachedMergeNodeNotLast = true;
                        return;
                    }
                }
            }
            var newStoreState = StoreDynamicDataStateProvider.GetMatchingStateProvider(currentNode);
            if (newStoreState != null)
            {
                stateProviders.AddNewProvider(newStoreState);
            }
            IDynamicDataLoadNode loadNode = currentNode as IDynamicDataLoadNode;
            if (loadNode != null)
            {
                foreach (var storeNode in stateProviders.MatchLoadToStore(currentNode))
                {
                    currentNode.DataFlowBackRelated.AddTwoWay(storeNode, loadNode.DataFlowDataProdivderIndex);
                }
            }
            reachedMergeNodeNotLast = false;
        }

    }
}
