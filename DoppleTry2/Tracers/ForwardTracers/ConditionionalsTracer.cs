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
            var mergeNonMerged = instructionNodes.Where(x => x.BranchProperties.MergingNodeProperties.MergedBranches.Any(y => y.BranchType != BranchType.SplitMerge));
            if (mergeNonMerged.Any())
            {
                //throw new Exception("Should merge only split merge branches");
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
            foreach (var mergingNode in instructionNodes.Where(x => x.BranchProperties.MergingNodeProperties.IsMergingNode))
            {
                foreach (var mergedBranch in mergingNode.BranchProperties.MergingNodeProperties.MergedBranches)
                {
                    mergedBranch.AddTwoWay(mergingNode);
                    mergedBranch.BranchType = BranchType.SplitMerge;
                }
            }
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
     
        private void MoveForwardAndMarkBranches(InstructionNode currentNode, List<BranchID> currentBranches = null, List<InstructionNode> visited = null)
        {
            if (visited ==null)
            {
                visited = new List<InstructionNode>();
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
                if (visited.Count(x => x ==currentNode) > 2 || ((currentNode is ConditionalJumpNode) && visited.Contains(currentNode)))
                {
                    throw new Exception("we shouldn't get to a visited node without passing an originating split node");
                }
                visited.Add(currentNode);
                currentBranches.ForEach(x => x.AddTwoWay(currentNode));
                var mergedBranches =  currentNode.BranchProperties.Branches.GroupBy(x => x.OriginatingNode).Where(x => x.Count() > 1);
                var mergedBranchesFlattened = mergedBranches.SelectMany(x => x).ToList();
                foreach(var mergedBranch in mergedBranchesFlattened)
                {
                    if (mergedBranch.MergingNode != null)
                    {
                        continue;
                    }
                    Console.WriteLine("node {0} is merge of branch {1} that originates in {2}", currentNode.InstructionIndex + "  " +currentNode.Instruction.ToString(), mergedBranch.Index, mergedBranch.OriginatingNode.InstructionIndex);
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
                var currentBranchesClone = new List<BranchID>(currentBranches);
                currentBranchesClone.Add(newBranch);
                MoveForwardAndMarkBranches(forwardNode, currentBranchesClone, new List<InstructionNode>(visited));
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
