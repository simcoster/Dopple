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
            var conditionalJumpNodes = instructionNodes.Where(x => x is ConditionalJumpNode).Cast<ConditionalJumpNode>();
            foreach (var conditionalJumpNode in conditionalJumpNodes)
            {
                PairedBranchIndex pairedBranchIndex = PairedBranchIndex.First;
                foreach (var forwardNode in conditionalJumpNode.ProgramFlowForwardRoutes)
                {
                    var branch = new BranchID(conditionalJumpNode) { BranchType = BranchType.Exit, PairedBranchesIndex = pairedBranchIndex };
                    conditionalJumpNode.CreatedBranches.Add(branch);
                    MoveForwardAndMarkBranch(conditionalJumpNode, forwardNode,branch);
                    pairedBranchIndex = PairedBranchIndex.Second;
                }
            }
        }

        private void MoveForwardAndMarkBranch(ConditionalJumpNode conditionalJumpNode,InstructionNode currentNode, BranchID currentBranch, List<InstructionNode> visited = null)
        {
            if (visited ==null)
            {
                visited = new List<InstructionNode>() { conditionalJumpNode };
            }
            while(true)
            {
                if (currentNode == conditionalJumpNode)
                {
                    currentNode.BranchProperties.Branches.Add(currentBranch);
                    currentBranch.BranchType = BranchType.Loop;
                    return;
                }
                if (visited.Contains(currentNode))
                {
                    return;
                }
                visited.Add(currentNode);
                BranchID secondBranch = currentNode.BranchProperties.Branches.FirstOrDefault(x => x.OriginatingNode == conditionalJumpNode && x != currentBranch);
                if (secondBranch != null)
                {
                    MarkMergeNode(currentNode, currentBranch, secondBranch);
                    MoveForwardAndRemoveBranch(currentNode, secondBranch);
                    return;
                }
                currentNode.BranchProperties.Branches.Add(currentBranch);
                if (currentNode.ProgramFlowForwardRoutes.Count ==0)
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
                MoveForwardAndMarkBranch(conditionalJumpNode, forwardNode, currentBranch, new List<InstructionNode>(visited));
            }
        }

        private void MoveForwardAndRemoveBranch(InstructionNode currentNode, BranchID branchToRemove, List<InstructionNode> visited = null)
        {
            if (visited == null)
            {
                visited = new List<InstructionNode>() { };
            }
            while (true)
            {
                if (visited.Contains(currentNode))
                {
                    return;
                }
                visited.Add(currentNode);
                currentNode.BranchProperties.Branches.RemoveAll(x => x == branchToRemove);
                if (currentNode.ProgramFlowForwardRoutes.Count != 1)
                {
                    break;
                }
                currentNode = currentNode.ProgramFlowForwardRoutes[0];
            }
            foreach (var forwardNode in currentNode.ProgramFlowForwardRoutes)
            {
                MoveForwardAndRemoveBranch(forwardNode, branchToRemove, visited);
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
        }
    }   
}
