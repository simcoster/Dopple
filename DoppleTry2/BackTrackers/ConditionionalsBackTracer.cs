using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionWrappers;
using Mono.Cecil.Cil;
using System.Diagnostics;

namespace DoppleTry2.BackTrackers
{
    class ConditionionalsBackTracer : BackTracer
    {
        public ConditionionalsBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        public override Code[] HandlesCodes
        {
            get
            {
                return CodeGroups.CondJumpCodes;
            }
        }

        public override void AddBackDataflowConnections(InstructionWrapper currentInst)
        {
            return;
            try
            {
                var targerInst = (Instruction) currentInst.Instruction.Operand;
                InstructionWrapper targetInstWrapper = InstructionWrappers.First(x => x.Instruction == targerInst);
                bool isLoopCondition = targerInst.Offset < currentInst.Instruction.Offset;
                if (isLoopCondition)
                {
                    foreach (var loopedNode in GetLoopedNodes(targetInstWrapper, currentInst))
                    {
                        loopedNode.BackDataFlowRelated.AddTwoWay(currentInst);
                    }
                }
                else
                {
                    int ifClauseFinalIndex;
                    InstructionWrapper prevInstWrapper = InstructionWrappers[InstructionWrappers.IndexOf(targetInstWrapper) - 1];
                    bool hasElse = prevInstWrapper.Instruction.OpCode.Code == Code.Br;
                    if (hasElse)
                    {
                        var jumpToInst = ((Instruction) prevInstWrapper.Instruction.Operand);
                        ifClauseFinalIndex = 
                    }
                }
            }
            catch
            {
                Debugger.Break();
            }
        }

        public List<InstructionWrapper> GetLoopedNodes (InstructionWrapper startNode, InstructionWrapper loopEnd)
        {
            var nodesToReturn = new List<InstructionWrapper>();
            InstructionWrapper currentNode = startNode;
            while(currentNode != loopEnd)
            {
                nodesToReturn.Add(currentNode);
                if (currentNode.ForwardProgramFlow.Count == 1)
                {
                    currentNode = currentNode.ForwardProgramFlow[0];
                }
                else
                {
                    List<InstructionWrapper> splitPathsNodes = currentNode.ForwardProgramFlow.SelectMany(x => GetLoopedNodes(x, loopEnd)).ToList();
                    return nodesToReturn.Concat(splitPathsNodes).Distinct().ToList();
                }
            }
            return nodesToReturn;
        }

        public List<InstructionWrapper> GetConditionedNodes(InstructionWrapper conditionNode)
        {
            if (conditionNode.Instruction)
        }

        private List<InstructionWrapper> GetForwardPath(InstructionWrapper startNode, List<InstructionWrapper> visited = null)
        {
            if (visited == null)
            {
                visited = new List<InstructionWrapper>();
            }
            InstructionWrapper currentNode = startNode;
            while(true)
            {
                if (visited.Contains(currentNode))
                {
                    return visited;
                }
                visited.Add(currentNode);
                if (currentNode.ForwardProgramFlow.Count ==0)
                {
                    return visited.Distinct().ToList();
                }
                else if (currentNode.ForwardProgramFlow.Count == 1)
                {
                    currentNode = currentNode.ForwardProgramFlow[0];
                }
                else
                {
                    IEnumerable<InstructionWrapper> subPaths = currentNode.ForwardProgramFlow.SelectMany(x => GetForwardPath(x, visited));
                    return visited.Concat(subPaths).Distinct().ToList();
                }
            }
        }
    }
}
