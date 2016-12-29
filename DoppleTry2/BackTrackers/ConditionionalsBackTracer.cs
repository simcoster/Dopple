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
                if (currentNode.ProgramFlowForwardRoutes.Count == 0)
                {
                    currentPath.Add(currentNode);
                    return new List<List<InstructionNode>>() { currentPath };
                }
                else if (currentPath.Contains(currentNode))
                {
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

        public override void AddBackDataflowConnections(InstructionNode currentInst)
        {
            if (allPossibleTracks == null)
            {
                allPossibleTracks = MapAllPossibleExecutionRoutes(InstructionNodes[0]);
            }
            var relevantTracks = allPossibleTracks.Where(x => x.Contains(currentInst)).ToList();
            foreach(var relevantTrack in relevantTracks)
            {
                RotateList(relevantTrack,currentInst);
            }
            var sharedNodes = relevantTracks[0];
            foreach(var relevantTrack in relevantTracks.Skip(1))
            {
                sharedNodes = sharedNodes.Intersect(relevantTrack).ToList();
            }
        }

        private void RotateList(List<InstructionNode> relevantTrack, InstructionNode currentInst)
        {
            throw new NotImplementedException();
        }

        private List<InstructionNode> SearchForInConditionNodes(InstructionNode conditionNode)
        {
            // either you end with ret or with a node with number of avaiable tracks back to you
            int routeCount = 0;
            while (true)
            {
            }
        }
    }
}
