using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;
using Mono.Cecil.Cil;
using System.Diagnostics;
using Dopple.BranchPropertiesNS;
using System.Collections.Concurrent;

namespace Dopple.BackTracers
{
    class ConditionionalsTracer
    {
        private Dictionary<int,BranchID> AllBrances;
        public void TraceConditionals(List<InstructionNode> instructionNodes)
        {
            instructionNodes.ForEach(x => x.BranchProperties.FirstInLoopOf = null);
            AllBrances = new Dictionary<int, BranchID>();
            var splitNodes = instructionNodes.Where(x => x is ConditionalJumpNode).Cast<ConditionalJumpNode>();
            var branchesSameOrigins = new List<List<BranchID>>();
            foreach (ConditionalJumpNode splitNode in splitNodes)
            {
                //do this once for better performance
                //TOOD remove
                foreach (var forwardNode in splitNode.ProgramFlowForwardRoutes.ToList())
                {
                    var branch = new BranchID(splitNode) { BranchType = BranchType.Exit};
                    AllBrances.Add(branch.Index, branch);
                    splitNode.CreatedBranches.Add(branch);
                    MoveForwardAndMarkBranch(splitNode, forwardNode,branch);
                }
                branchesSameOrigins.Add(splitNode.CreatedBranches);
            }
            var nodesToRemoveFromBranches = new Dictionary<BranchID, ConcurrentBag<InstructionNode>>();
            foreach(var branch in AllBrances.Values)
            {
                nodesToRemoveFromBranches.Add(branch, new ConcurrentBag<InstructionNode>());
            } 
            Parallel.ForEach((instructionNodes), node =>
            {
                foreach (var branchesSameOrigin in branchesSameOrigins)
                {
                    var nodeBranchesSameOrigin = branchesSameOrigin.Where(x => node.BranchProperties.Branches.Contains(x)).ToList();
                    if (nodeBranchesSameOrigin.Count > 1)
                    {
                        foreach (var nodeBranchToRemove in nodeBranchesSameOrigin)
                        {
                            nodesToRemoveFromBranches[nodeBranchToRemove].Add(node);
                            node.BranchProperties.Branches.Remove(nodeBranchToRemove);
                        }
                    }
                }
            });
            Parallel.ForEach((nodesToRemoveFromBranches.Keys), branch =>
            {
                foreach(var node in nodesToRemoveFromBranches[branch])
                {
                    branch.BranchNodes.Remove(node);
                }
            });
        }

        private void MoveForwardAndMarkBranch(InstructionNode originNode,InstructionNode currentNode, BranchID currentBranch, HashSet<InstructionNode> visited = null)
        {
            if (visited ==null)
            {
                visited = new HashSet<InstructionNode>() { originNode };
            }
            while(true)
            {
                if (currentNode == originNode)
                {
                    currentNode.BranchProperties.Branches.AddDistinct(currentBranch);
                    currentBranch.BranchNodes.Add(currentNode);
                    currentBranch.BranchType = BranchType.Loop;
                    currentBranch.BranchNodes[0].BranchProperties.FirstInLoopOf = currentBranch;
                    return;
                }
                if (visited.Contains(currentNode))
                {
                    return;
                }
                visited.Add(currentNode);
                if (currentBranch.MergingNode == null)
                {
                    List<BranchID> otherBranches = currentNode.BranchProperties.Branches.Where(x => x.OriginatingNode == originNode && x != currentBranch && x.MergingNode == null).ToList();
                    if (otherBranches.Count > 0)
                    {
                        int i = 1;
                        foreach (var mergedBranch in otherBranches.Concat(new[] { currentBranch }).ToList())
                        {
                            mergedBranch.BranchType = BranchType.SplitMerge;
                            MarkMergeNode(currentNode, mergedBranch);
                            mergedBranch.PairedBranchesIndex = i;
                            i++;
                        }

                    }
                }
                currentNode.BranchProperties.Branches.AddDistinct(currentBranch);
                currentBranch.BranchNodes.Add(currentNode);
                if (currentNode.ProgramFlowForwardRoutes.Count == 0)
                {
                    return;
                }
                if (currentNode.ProgramFlowForwardRoutes.Count > 1)
                {
                    break;
                }
                currentNode = currentNode.ProgramFlowForwardRoutes[0];
            }
            foreach (var forwardNode in currentNode.ProgramFlowForwardRoutes)
            {
                MoveForwardAndMarkBranch(originNode, forwardNode, currentBranch, visited);
            }
        }

        private static void MarkMergeNode(InstructionNode currentNode, BranchID mergedBranch)
        {
            currentNode.BranchProperties.MergingNodeProperties.IsMergingNode = true;
            currentNode.BranchProperties.MergingNodeProperties.MergedBranches.Add(mergedBranch);
            mergedBranch.MergingNode = currentNode;
        }
    }   
}
