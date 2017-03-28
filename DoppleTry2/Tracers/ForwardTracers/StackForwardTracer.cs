using Dopple.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.BackTracers
{
    public class StackForwardTracer
    {
        public void TraceForward(List<InstructionNode> instructionNodes)
        {
            TraceForwardRec(instructionNodes);
            foreach (var node in instructionNodes)
            {
                node.DataFlowBackRelated.UpdateLargestIndex();
                node.StackBacktraceDone = true;
            }
        }
        public void TraceForwardRec(List<InstructionNode> instructionNodes, InstructionNode currentNode = null, List<InstructionNode> visitedNodes = null , Stack<InstructionNode> stackedNodes =null)
        {
            if (stackedNodes == null && visitedNodes == null)
            {
                stackedNodes = new Stack<InstructionNode>();
                visitedNodes = new List<InstructionNode>();
                currentNode = instructionNodes[0];
            }
            else if (!(stackedNodes !=null && visitedNodes != null))
            {
                throw new Exception("all should be null or none");
            }
            lock(currentNode)
            {
                if (!currentNode.StackBacktraceDone)
                {
                    currentNode.DataFlowBackRelated.ResetIndex();
                    for (int i = 0; i < currentNode.StackPopCount; i++)
                    {
                        if (stackedNodes.Count == 0)
                        {
                            throw new Exception("not enough stacked arguments");
                        }
                        currentNode.DataFlowBackRelated.AddTwoWay(stackedNodes.Pop());
                    }
                    for (int i = 0; i < currentNode.StackPushCount; i++)
                    {
                        stackedNodes.Push(currentNode);
                    }
                }
            }
           
            int forwardRouteCount = currentNode.ProgramFlowForwardRoutes.Count;
            if (forwardRouteCount == 0 || visitedNodes.Contains(currentNode))
            {
                if (stackedNodes.Count > 1)
                {
                    //TODO remove
                    //throw new StackPopException("Finished with extra nodes", visitedNodes.Concat(new[] { currentNode }).ToList());
                }
                return;
            }
            visitedNodes.Add(currentNode);

            if (forwardRouteCount == 1)
            {
                TraceForwardRec(instructionNodes, currentNode.ProgramFlowForwardRoutes[0], visitedNodes, stackedNodes);
            }
            else
            {
                Parallel.ForEach(currentNode.ProgramFlowForwardRoutes, (forwardRoute) =>
                 {
                     var stackedNodesClone = new Stack<InstructionNode>(stackedNodes.Reverse());
                     var visitedNodesClone = new List<InstructionNode>(visitedNodes);
                     TraceForwardRec(instructionNodes, forwardRoute, visitedNodesClone, stackedNodesClone);
                 });
            }
        }
    }
}
