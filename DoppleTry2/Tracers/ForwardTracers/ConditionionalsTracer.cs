using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;
using Mono.Cecil.Cil;
using System.Diagnostics;
using Dopple.BranchPropertiesNS;
using Dopple.Tracers.ForwardTracers;

namespace Dopple.BackTracers
{
    class ConditionionalsTracer
    {
        public void TraceConditionals(List<InstructionNode> instructionNodes)
        {
            var splitNodes = instructionNodes.Where(x => x is ConditionalJumpNode).Cast<ConditionalJumpNode>();
            var branchesSameOrigins = new List<List<BranchID>>();
            foreach (ConditionalJumpNode splitNode in splitNodes)
            {
                PairedBranchIndex pairedBranchIndex = PairedBranchIndex.First;
                foreach (var forwardNode in splitNode.ProgramFlowForwardRoutes.ToList())
                {
                    var branch = new BranchID(splitNode) { BranchType = BranchType.Exit, PairedBranchesIndex = pairedBranchIndex };
                    splitNode.CreatedBranches.Add(branch);
                    MoveForwardAndMarkBranch(splitNode, forwardNode,branch);
                    pairedBranchIndex = PairedBranchIndex.Second;
                }
                branchesSameOrigins.Add(splitNode.CreatedBranches);
            }
            foreach(var node in instructionNodes)
            {
                foreach(var branchesSameOrigin in branchesSameOrigins)
                {
                    if (branchesSameOrigin[0].MergingNode == node)
                    {
                        test this
                        continue;
                    }
                    if (branchesSameOrigin.All(x => node.BranchProperties.Branches.Contains(x)))
                    {
                        node.BranchProperties.Branches.RemoveAll(x => branchesSameOrigin.Contains(x));
                    }
                }
            }
        }

        private void MoveForwardAndMarkBranch(InstructionNode originNode,InstructionNode currentNode, BranchID currentBranch, List<InstructionNode> visited = null)
        {
            if (visited ==null)
            {
                visited = new List<InstructionNode>() { originNode };
            }
            while(true)
            {
                if (currentNode == originNode)
                {
                    currentNode.BranchProperties.Branches.AddDistinct(currentBranch);
                    currentBranch.BranchNodes.Add(currentNode);
                    currentBranch.BranchType = BranchType.Loop;
                    return;
                }
                if (visited.Contains(currentNode))
                {
                    return;
                }
                visited.Add(currentNode);
                BranchID secondBranch = currentNode.BranchProperties.Branches.FirstOrDefault(x => x.OriginatingNode == originNode && x != currentBranch);
                if (secondBranch != null)
                {
                    MarkMergeNode(currentNode, currentBranch, secondBranch);
                    return;
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

        private static void MarkMergeNode(InstructionNode currentNode, BranchID currentBranch, BranchID secondBranch)
        {
            currentNode.BranchProperties.Branches.Remove(secondBranch);
            currentNode.BranchProperties.MergingNodeProperties.IsMergingNode = true;
            currentNode.BranchProperties.MergingNodeProperties.MergedBranches.Add(secondBranch);
            currentNode.BranchProperties.MergingNodeProperties.MergedBranches.Add(currentBranch);
            currentBranch.PairedBranchesIndex = PairedBranchIndex.First;
            secondBranch.BranchType = BranchType.SplitMerge;
            currentBranch.PairedBranchesIndex = PairedBranchIndex.Second;
            currentBranch.BranchType = BranchType.SplitMerge;
            currentBranch.MergingNode = currentNode;
            secondBranch.MergingNode = currentNode;
        }
    }   
}
