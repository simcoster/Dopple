using DoppleTry2.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.BackTrackers
{
    public class StackForwardTracer
    {
        private List<InstructionNode> _InstructionNodes;

        public StackForwardTracer(List<InstructionNode> instructionNodes)
        {
            _InstructionNodes = instructionNodes;
        }
        public void TraceForward(InstructionNode currentNode)
        {
            TraceForwardRec(currentNode);
            _InstructionNodes.ForEach(x => x.DataFlowBackRelated.UpdateLargestIndex());
        }
        public void TraceForwardRec(InstructionNode currentNode, List<InstructionNode> visitedNodes = null , Stack<InstructionNode> stackedNodes =null)
        {
            if (stackedNodes == null && visitedNodes == null)
            {
                stackedNodes = new Stack<InstructionNode>();
                visitedNodes = new List<InstructionNode>();
                currentNode = _InstructionNodes[0];
            }
            else if (!(stackedNodes !=null && visitedNodes != null))
            {
                throw new Exception("all should be null or none");
            }            
            for (int i=0; i < currentNode.StackPopCount; i++)
            {
                currentNode.DataFlowBackRelated.AddTwoWay(stackedNodes.Pop());
            }
            for (int i = 0; i < currentNode.StackPushCount; i++)
            {
                stackedNodes.Push(currentNode);
            }
            int forwardRouteCount = currentNode.ProgramFlowForwardRoutes.Count;
            if (forwardRouteCount == 0 || visitedNodes.Contains(currentNode))
            {
                if (stackedNodes.Count > 1)
                {
                    //throw new Exception("someone didn't get everything");
                }
                return;
            }
            visitedNodes.Add(currentNode);

            if (forwardRouteCount == 1)
            {
                TraceForwardRec(currentNode.ProgramFlowForwardRoutes[0], visitedNodes, stackedNodes);
            }
            else
            {
                foreach(var forwardRoute in currentNode.ProgramFlowForwardRoutes)
                {
                    var stackedNodesClone = new Stack<InstructionNode>(stackedNodes.Reverse());
                    var visitedNodesClone = new List<InstructionNode>(visitedNodes);
                    _InstructionNodes.ForEach(x => x.DataFlowBackRelated.ResetIndex());
                    TraceForwardRec(forwardRoute, visitedNodesClone, stackedNodesClone);
                }
            }
        }

        private void MergeCloneIntoOriginal(List<InstructionNode> clonedInstructionNodes)
        {
            for (int i = 0; i < clonedInstructionNodes.Count; i++)
            {
                foreach (var backNode in clonedInstructionNodes[i].DataFlowBackRelated)
                {
                    int index = clonedInstructionNodes.IndexOf(backNode.Argument);
                    _InstructionNodes[i].DataFlowBackRelated.AddTwoWay(_InstructionNodes[index], backNode.ArgIndex);
                }
            }
        }
    }
}
