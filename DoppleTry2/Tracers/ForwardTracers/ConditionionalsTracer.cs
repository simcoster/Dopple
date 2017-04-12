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
        public void TraceConditionals(List<InstructionNode> instructionNodes)
        {
            var nodesToRemoveFromBranches = new Dictionary<BranchID, ConcurrentBag<InstructionNode>>();
            MoveForwardAndMarkBranch(instructionNodes[0]);
            foreach (var branch in BranchID.AllBranches)
            {
                nodesToRemoveFromBranches.Add(branch, new ConcurrentBag<InstructionNode>());
            }
            Parallel.ForEach((instructionNodes), node =>
            {
                foreach (var branchesSameOriginGroup in node.BranchProperties.Branches.GroupBy(x => x.OriginatingNode).Where(x => x.Count()>1).ToList())
                {
                    if (node.BranchProperties.MergingNodeProperties.MergedBranches.Contains(branchesSameOriginGroup.First()))
                    {
                        continue;
                    }
                    foreach(var branchToRemove in branchesSameOriginGroup)
                    {
                        nodesToRemoveFromBranches[branchToRemove].Add(node);
                        node.BranchProperties.Branches.Remove(branchToRemove);
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

        private void MoveForwardAndMarkBranch(InstructionNode currentNode, List<BranchID> accumelatedBrances =null, HashSet<InstructionNode> visited = null)
        {
            if (visited ==null)
            {
                visited = new HashSet<InstructionNode>() { currentNode};
                accumelatedBrances = new List<BranchID>();
            }
            while(true)
            {
                if (visited.Contains(currentNode))
                {
                    if (currentNode is ConditionalJumpNode)
                    {
                        var loopingBranch = accumelatedBrances.First(x => ((ConditionalJumpNode) currentNode).CreatedBranches.Contains(x));
                        loopingBranch.BranchType = BranchType.Loop;
                        loopingBranch.BranchNodes[0].BranchProperties.FirstInLoopOf = loopingBranch;
                    }
                    return;
                }
                visited.Add(currentNode);
                currentNode.BranchProperties.Branches.AddRangeDistinct(accumelatedBrances);
                foreach(var branchSameOriginGroup in currentNode.BranchProperties.Branches.GroupBy(x => x.OriginatingNode).Where(x => x.Count() > 1))
                {
                    if (branchSameOriginGroup.Any(x => x.MergingNode == null))
                    {
                        branchSameOriginGroup.ForEach(x => { x.MergingNode = currentNode; x.BranchType = BranchType.SplitMerge; });
                        currentNode.BranchProperties.MergingNodeProperties.IsMergingNode = true;
                        currentNode.BranchProperties.MergingNodeProperties.MergedBranches.AddRange(branchSameOriginGroup);
                    }
                }
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
                MoveForwardAndMarkBranch(forwardNode,accumelatedBrances.Concat(new[] { new BranchID(currentNode) }).ToList(), new HashSet<InstructionNode>(visited));
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
