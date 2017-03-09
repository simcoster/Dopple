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
    class ConditionionalsBackTracer : BackTracer
    {
        private List<List<InstructionNode>> MapAllPossibleExecutionRoutes(InstructionNode startNode, List<InstructionNode> startPath = null)
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
                else if (currentNode.ProgramFlowForwardRoutes.Count == 1)
                {
                    currentPath.Add(currentNode);
                    currentNode = currentNode.ProgramFlowForwardRoutes[0];
                }
                else
                {
                    currentPath.Add(currentNode);
                    foreach (var forwardNode in currentNode.ProgramFlowForwardRoutes)
                    {
                        possibleExecutionRoutes.AddRange(MapAllPossibleExecutionRoutes(forwardNode, currentPath));
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

        protected override void BackTraceDataFlowSingle(InstructionNode currentNode)
        {
            foreach (var executionTrack in GetNodesInClosedCondition(currentNode))
            {
                foreach(var node in executionTrack.Nodes)
                {
                    IndexedArgument trackArg = new IndexedArgument((int) executionTrack.TrackType, currentNode,node.ProgramFlowBackAffected);
                    if (!node.ProgramFlowBackAffected.Any(x => x.Argument == trackArg.Argument && x.ArgIndex <= trackArg.ArgIndex ))
                    {
                        node.ProgramFlowBackAffected.AddTwoWay(trackArg); 
                    }
                }
            }
        }

        private List<ExecutionTrack> GetNodesInClosedCondition(InstructionNode currentNode)
        {
            var relevantTracks = MapAllPossibleExecutionRoutes(currentNode).Select(x => x.Skip(1).ToList()).ToList();
            List<InstructionNode> sharedNodes = relevantTracks.Aggregate((x, y) => x.Intersect(y).ToList()).ToList();
            IEnumerable<InstructionNode> nodesInCondition;
            List<ExecutionTrack> executionTracks = new List<ExecutionTrack>();
            if (sharedNodes.Count > 0)
            {
                InstructionNode conditionEndNode = sharedNodes.First();
                nodesInCondition = relevantTracks
                                          .Select(x => x.TakeWhile(y => y != conditionEndNode))
                                          .Aggregate((x, y) => x.Concat(y))
                                          .Distinct().ToList();
                executionTracks.Add(new ExecutionTrack(nodesInCondition.ToList(),TrackType.ClosedBranch));
                return executionTracks;
            }

            var loopTracks = relevantTracks.Where(x => x.Contains(currentNode)).ToList();
            if (loopTracks.Count >0)
            {
                executionTracks.AddRange(loopTracks.Select(x => new ExecutionTrack(x, TrackType.Loop)));
            }
            var retTracks = relevantTracks.Where(x => x.Any(y => y is RetInstructionNode));
            executionTracks.AddRange(retTracks.Select(x => new ExecutionTrack(x, TrackType.RetBranch)));
            return executionTracks;
        }
    }

    internal class ExecutionTrack
    {
        internal ExecutionTrack(List<InstructionNode> nodes, TrackType trackType)
        {
            Nodes = nodes;
            TrackType = trackType;
        }
        internal List<InstructionNode> Nodes { get; private set; }
        internal TrackType TrackType { get; private set; }
    }

    public enum TrackType
    {
        Loop =1,
        ClosedBranch=2,
        RetBranch=3
    }
}
