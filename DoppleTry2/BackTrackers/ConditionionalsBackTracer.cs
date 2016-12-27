using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionNodes;
using Mono.Cecil.Cil;
using System.Diagnostics;

namespace DoppleTry2.BackTrackers
{
    class ConditionionalsBackTracer : BackTracer
    {
        public ConditionionalsBackTracer(List<InstructionNode> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        public override Code[] HandlesCodes
        {
            get
            {
                return CodeGroups.CondJumpCodes;
            }
        }

        public override void AddBackDataflowConnections(InstructionNode currentInst)
        {
            var targerInst = (Instruction) currentInst.Instruction.Operand;
            InstructionNode targetInstWrapper = InstructionWrappers.First(x => x.Instruction == targerInst);
            bool isLoopCondition = targerInst.Offset < currentInst.Instruction.Offset;
            if (isLoopCondition)
            {
                foreach (var loopedNode in GetLoopedNodes(targetInstWrapper, currentInst))
                {
                    loopedNode.ProgramFlowBackAffected.AddTwoWay(currentInst);
                }
            }
            else
            {
                foreach (var conditionedNode in GetConditionedNodes(currentInst, targetInstWrapper))
                {
                    conditionedNode.ProgramFlowBackAffected.AddTwoWay(currentInst);
                }
            }
        }

        public List<InstructionNode> GetLoopedNodes (InstructionNode startNode, InstructionNode loopEnd)
        {
            var nodesToReturn = new List<InstructionNode>();
            InstructionNode currentNode = startNode;
            while(currentNode != loopEnd)
            {
                nodesToReturn.Add(currentNode);
                if (currentNode.ProgramFlowForwardRoutes.Count == 1)
                {
                    currentNode = currentNode.ProgramFlowForwardRoutes[0];
                }
                else
                {
                    List<InstructionNode> splitPathsNodes = currentNode.ProgramFlowForwardRoutes.SelectMany(x => GetLoopedNodes(x, loopEnd)).ToList();
                    return nodesToReturn.Concat(splitPathsNodes).Distinct().ToList();
                }
            }
            return nodesToReturn;
        }

        public IEnumerable<InstructionNode> GetConditionedNodes(InstructionNode conditionNode, InstructionNode targetInstWrapper)
        {
            int ifClauseFinalIndex;
            InstructionNode prevInstWrapper = InstructionWrappers[InstructionWrappers.IndexOf(targetInstWrapper) - 1];
            bool hasElse = prevInstWrapper.Instruction.OpCode.Code == Code.Br;
            if (hasElse)
            {
                ifClauseFinalIndex = InstructionWrappers.First(x => x.Instruction == ((Instruction) prevInstWrapper.Instruction.Operand)).InstructionIndex;
            }
            else
            {
                ifClauseFinalIndex = targetInstWrapper.InstructionIndex;
            }
            for (int i = conditionNode.InstructionIndex; i < ifClauseFinalIndex; i++)
            {
                yield return InstructionWrappers[i];
            }
        }

        private List<InstructionNode> GetForwardPath(InstructionNode startNode, List<InstructionNode> visited = null)
        {
            if (visited == null)
            {
                visited = new List<InstructionNode>();
            }
            InstructionNode currentNode = startNode;
            while(true)
            {
                if (visited.Contains(currentNode))
                {
                    return visited;
                }
                visited.Add(currentNode);
                if (currentNode.ProgramFlowForwardRoutes.Count ==0)
                {
                    return visited.Distinct().ToList();
                }
                else if (currentNode.ProgramFlowForwardRoutes.Count == 1)
                {
                    currentNode = currentNode.ProgramFlowForwardRoutes[0];
                }
                else
                {
                    IEnumerable<InstructionNode> subPaths = currentNode.ProgramFlowForwardRoutes.SelectMany(x => GetForwardPath(x, visited));
                    return visited.Concat(subPaths).Distinct().ToList();
                }
            }
        }
    }
}
