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
            MoveForwardAndMarkBranches(instructionNodes[0]);
            RemoveMutlipleBranchesSameOrigin(instructionNodes);
            if (instructionNodes.Any(x => x.BranchProperties.MergingNodeProperties.MergedBranches.Any(y => y.BranchType != BranchType.SplitMerge)))
            {
                throw new Exception("Should merge only split merge branches");
            }
            MergeReturnNodes(instructionNodes);
        }

        private void RemoveMutlipleBranchesSameOrigin(List<InstructionNode> instructionNodes)
        {
            var mergedBranchesGroups = BranchID.AllBranches.GroupBy(x => x.OriginatingNode).Where(x => x.Count()>1).ToList();
            //Parallel.ForEach((instructionNodes), node =>
            foreach (var node in instructionNodes)
            {
                foreach (var branchesSameOrigin in mergedBranchesGroups)
                {
                    var mergedBranchesInNode = branchesSameOrigin.Intersect(node.BranchProperties.Branches).ToList();
                    if (mergedBranchesInNode.Count > 1)
                    {
                        foreach(var  mergedBranchInNode in  mergedBranchesInNode)
                        {
                            node.BranchProperties.Branches.Remove(mergedBranchInNode);
                            mergedBranchInNode.BranchNodes.Remove(node);
                        }
                    }
                }
            }
            Parallel.ForEach((instructionNodes.Where(x => x.BranchProperties.MergingNodeProperties.IsMergingNode)), mergingNode =>
            {
                foreach (var mergedBranch in mergingNode.BranchProperties.MergingNodeProperties.MergedBranches)
                {
                    mergedBranch.AddTwoWay(mergingNode);
                }
            });
        }

        private void MergeReturnNodes(List<InstructionNode> instructionNodes)
        {
            if (instructionNodes[0].InliningProperties.Inlined)
            {
                var returnNodes = instructionNodes.Where(x => x.Instruction.OpCode.Code == Code.Ret);
                var retNodeToKeep = returnNodes.FirstOrDefault();
                if (retNodeToKeep == null)
                {
                    return;
                }
                foreach (var returnNodeToMerge in returnNodes.Skip(1).ToList())
                {
                    returnNodeToMerge.MergeInto(retNodeToKeep, false);
                    instructionNodes.Remove(returnNodeToMerge);
                }
                foreach(var exitBranch in BranchID.AllBranches.Where(x => x.BranchType == BranchType.Exit))
                {
                    exitBranch.BranchType = BranchType.SplitMerge;
                    exitBranch.MergingNode = retNodeToKeep;
                    retNodeToKeep.BranchProperties.MergingNodeProperties.IsMergingNode = true;
                    retNodeToKeep.BranchProperties.MergingNodeProperties.MergedBranches.Add(exitBranch);
                }
            }
        }
     
        private void MoveForwardAndMarkBranches(InstructionNode currentNode, List<BranchID> currentBranches = null, HashSet<InstructionNode> visited = null)
        {
            if (visited ==null)
            {
                visited = new HashSet<InstructionNode>();
                currentBranches = new List<BranchID>();
            }
            while (true)
            {
                var loopBranches = currentBranches.Where(x => x.OriginatingNode == currentNode).ToList();
                if (loopBranches.Count >1)
                {
                    throw new Exception("only 1 branch should be loop");
                }
                if (loopBranches.Count == 1)
                {
                    loopBranches.First().BranchType = BranchType.Loop;
                    loopBranches.First().BranchNodes[0].BranchProperties.FirstInLoop = true;
                    return;
                }
                if (visited.Contains(currentNode))
                {
                    throw new Exception("we shouldn't get to a visited node without passing an originating split node");
                }
                visited.Add(currentNode);
                currentNode.BranchProperties.Branches.AddRangeDistinct(currentBranches);
                var mergedBranches =  currentNode.BranchProperties.Branches.Concat(currentBranches).GroupBy(x => x.OriginatingNode).Where(x => x.Count() > 1);
                foreach(var mergedBranch in mergedBranches.SelectMany(x => x).Where(x => x.MergingNode == null))
                {
                    MarkMergeNode(currentNode, mergedBranch);
                    mergedBranch.PairedBranchesIndex = currentNode.BranchProperties.MergingNodeProperties.MergedBranches.IndexOf(mergedBranch);
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
                var newBranch = CreateNewBranch((ConditionalJumpNode)currentNode);
                currentBranches.Add(newBranch);
                MoveForwardAndMarkBranches(forwardNode, new List<BranchID>(currentBranches));
            }
        }

        private static BranchID CreateNewBranch(ConditionalJumpNode splitNode)
        {
            BranchID newBranch = new BranchID(splitNode);
            splitNode.CreatedBranches.Add(newBranch);
            return newBranch;
        }

        private static void MarkMergeNode(InstructionNode currentNode, BranchID mergedBranch)
        {
            currentNode.BranchProperties.MergingNodeProperties.IsMergingNode = true;
            currentNode.BranchProperties.MergingNodeProperties.MergedBranches.Add(mergedBranch);
            mergedBranch.MergingNode = currentNode;
        }
    }   
}
