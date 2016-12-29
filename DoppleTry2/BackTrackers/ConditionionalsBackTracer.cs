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
        private List<List<InstructionNode>> allPossibleTracks;

        public ConditionionalsBackTracer(List<InstructionNode> instructionNodes) : base(instructionNodes)
        {
        }

        private List<List<InstructionNode>> MapAllPossibleExecutionRoutes(InstructionNode startNode,  List<InstructionNode> startPath = null)
        {
            List<InstructionNode> currentPath;
            if (startPath == null)
            {
                currentPath = new List<InstructionNode>();
            }
            else
            {
                currentPath = new List<InstructionNode>(startPath);
            }
            var possibleExecutionRoutes = new List<List<InstructionNode>>();
            var currentNode = startNode;
            while (true)
            {
                if (currentNode.ProgramFlowForwardRoutes.Count == 0 || currentPath.Contains(currentNode))
                {
                    currentPath.Add(currentNode);
                    return new List<List<InstructionNode>>() { currentPath };
                }
                else if (currentNode.ProgramFlowForwardRoutes.Count ==1)
                {
                    currentPath.Add(currentNode);
                    currentNode = currentNode.ProgramFlowForwardRoutes[0];
                }
                else
                {
                    currentPath.Add(currentNode);
                    foreach(var path in currentNode.ProgramFlowForwardRoutes)
                    {
                        possibleExecutionRoutes.AddRange(MapAllPossibleExecutionRoutes(path, currentPath));
                    }
                    return possibleExecutionRoutes;
                }
            }
        }

        public override Code[] HandlesCodes
        {
            get
            {
                return CodeGroups.CondJumpCodes;
            }
        }

        public override void AddBackDataflowConnections(InstructionNode currentNode)
        {
            var relevantTracks = MapAllPossibleExecutionRoutes(currentNode).Select(x => x.Skip(1));
            List<InstructionNode> sharedNodes = relevantTracks.Aggregate((x, y) => x.Intersect(y).ToList()).ToList();
            IEnumerable<InstructionNode> nodesInCondition;
            if (sharedNodes.Count > 0)
            {
                InstructionNode conditionEndNode = sharedNodes.First();
                nodesInCondition = relevantTracks
                                          .Select(x => x.TakeWhile(y => y != conditionEndNode))
                                          .Aggregate((x, y) => x.Concat(y))
                                          .Distinct();
            }
            else
            {
                try
                {
                    var loopTracks = relevantTracks.Where(x => x.Contains(currentNode)).ToList();
                    if (loopTracks.Count > 1)
                    {
                        nodesInCondition = loopTracks.Aggregate((x, y) => x.Concat(y)).Distinct();
                    }
                    else
                    {
                        nodesInCondition = loopTracks[0];
                    }
                }
              catch
                {
                    nodesInCondition = new List<InstructionNode>();
                    Debug.WriteLine("problem with current node");
                }
            }
          
            foreach(var node in nodesInCondition)
            {
                node.ProgramFlowBackAffected.AddTwoWay(currentNode);
            }
        }
    }
}
