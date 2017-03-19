using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;
using Mono.Cecil.Cil;
using System.Diagnostics;

namespace Dopple.BackTracers
{
    class ConditionionalsTracer
    {
        public void TraceConditionals(List<InstructionNode> instructionNodes)
        {
            var conditionalJumpNodes = instructionNodes.Where(x => x is ConditionalJumpNode).Cast<ConditionalJumpNode>();
            foreach (var conditionalJumpNode in conditionalJumpNodes)
            {
                int i = 0;
                foreach (var forwardNode in conditionalJumpNode.ProgramFlowForwardRoutes)
                {
                    MoveForwardAndMarkBranch(conditionalJumpNode, forwardNode, i);
                    i++;
                }
                foreach (var node in instructionNodes)
                {
                    bool bothBranchesReachThisNode = node.ProgramFlowBackAffected.Count(x => x.Argument == conditionalJumpNode) > 1;
                    if (bothBranchesReachThisNode)
                    {
                        node.ProgramFlowBackAffected.RemoveAllTwoWay(x => x.Argument == conditionalJumpNode);
                    }
                }
            }
        }

        private void MoveForwardAndMarkBranch(ConditionalJumpNode conditionalJumpNode,InstructionNode currentNode, int index, List<InstructionNode> visited = null)
        {
            if (visited ==null)
            {
                visited = new List<InstructionNode>();
            }
            if (visited.Contains(currentNode))
            {
                return;
            }
            visited.Add(currentNode);
            currentNode.ProgramFlowBackAffected.AddTwoWay(conditionalJumpNode, index);
            if (currentNode == conditionalJumpNode)
            {
                return;
            }
            foreach (var forwardNode in currentNode.ProgramFlowForwardRoutes)
            {
                MoveForwardAndMarkBranch(conditionalJumpNode, forwardNode, index, visited);
            }
        }
    }   
}
