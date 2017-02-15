using DoppleTry2;
using DoppleTry2.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarityByMatching
{
    public class GraphSimilarityCalc
    {
        private static readonly CodeInfo opCodeInfo = new CodeInfo();
      
        public static NodePairings GetDistance(List<InstructionNode> firstGraph, List<InstructionNode> secondGraph)
        {
            List<LabeledVertex> firstGraphLabeled = GetLabeled(firstGraph);
            List<LabeledVertex> secondGraphLabeled = GetLabeled(secondGraph);
            NodePairings bestMatch = GetPairings(firstGraphLabeled, secondGraphLabeled);
            for (int i = 0; i < 10; i++)
            {
                NodePairings pairing = GetPairings(firstGraphLabeled, secondGraphLabeled);
                if (pairing.Score > bestMatch.Score)
                {
                    bestMatch = pairing;
                }
            }
            return bestMatch ;
        }

        private static NodePairings GetPairings(List<LabeledVertex> firstGraphLabeled, List<LabeledVertex> secondGraphLabeled)
        {
            NodePairings nodePairing = new NodePairings(firstGraphLabeled, secondGraphLabeled);
            var pairings = nodePairing.Pairings;
            nodePairing.FirstGraph = firstGraphLabeled;
            nodePairing.SecondGraph = secondGraphLabeled;

            Random rnd = new Random();
            foreach (var firstGraphVertex in firstGraphLabeled.OrderBy(x => rnd.Next()))
            {
                var vertexPossiblePairings = new Dictionary<LabeledVertex, int>();
                var secondGraphCandidates = secondGraphLabeled.Where(x => CodeGroups.AreSameGroup(x.Opcode, firstGraphVertex.Opcode)).ToList();
                foreach (var secondGraphCandidate in secondGraphCandidates.OrderBy(x => rnd.Next()))
                {
                    vertexPossiblePairings.Add(secondGraphCandidate, GetScore(firstGraphVertex, secondGraphCandidate, nodePairing));
                }
                var winningPairs = vertexPossiblePairings.Where(x=> x.Value >0).GroupBy(x => x.Value).OrderByDescending(x => x.Key).FirstOrDefault();
                if (winningPairs != null)
                {
                    KeyValuePair<LabeledVertex, int> winningPair = winningPairs.ElementAt(rnd.Next(0, winningPairs.Count()));
                    nodePairing.Score += winningPair.Value;
                    pairings[winningPair.Key].Add(new SingleNodePairing(firstGraphVertex, winningPair.Value));
                }
                else
                {
                    nodePairing.Score -= VertexScorePoints.VertexExactMatch;
                }
            }
            return nodePairing;
        }

        private static int GetScore(LabeledVertex firstGraphVertex, LabeledVertex secondGraphVertex, NodePairings pairings)
        {
            int score = 0;
            if (secondGraphVertex.Opcode == firstGraphVertex.Opcode)
            {
                score += VertexScorePoints.VertexExactMatch;
            }
            else
            {
                score += VertexScorePoints.VertexCodeFamilyMatch;
            }
            var backEdgeScore = EdgeScorer.ScoreEdges(firstGraphVertex.BackEdges, secondGraphVertex.BackEdges, pairings, SharedSourceOrDest.Dest);
            var forwardEdgeScore = EdgeScorer.ScoreEdges(firstGraphVertex.ForwardEdges, secondGraphVertex.ForwardEdges, pairings, SharedSourceOrDest.Source);
            score += backEdgeScore + forwardEdgeScore;
            if (pairings.Pairings[secondGraphVertex].Count > 0)
            {
                score -= VertexScorePoints.SingleToMultipleVertexMatchPenalty;
            }
            return score;
        }
        private static List<LabeledVertex> GetLabeled(List<InstructionNode> graph)
        {
            var labeledVertexes = new List<LabeledVertex>();
            foreach (var instructionNode in graph)
            {
                LabeledVertex vertex = new LabeledVertex();
                vertex.Opcode = instructionNode.Instruction.OpCode.Code;
                vertex.Operand = instructionNode.Instruction.Operand;
                vertex.Index = instructionNode.InstructionIndex;
                vertex.Method = instructionNode.Method;
                labeledVertexes.Add(vertex);
            }
            AddEdges(graph, labeledVertexes);
            return labeledVertexes;
        }

        private static List<LabeledVertex> GetBackSingleUnitTree(LabeledVertex frontMostInSingleUnit)
        {
            List<LabeledVertex> backTree = new List<LabeledVertex>();
            Queue<LabeledVertex> backRelated = new Queue<LabeledVertex>();
            foreach (var backSingleUnit in frontMostInSingleUnit.BackEdges.Where(x => x.EdgeType == EdgeType.SingleUnit))
            {
                backRelated.Enqueue(backSingleUnit.SourceVertex);
            }
            while (backRelated.Count > 0)
            {
                var currNode = backRelated.Dequeue();
                backTree.Add(currNode);
                foreach(var backSingleUnit in currNode.BackEdges.Where(x => x.EdgeType == EdgeType.SingleUnit))
                {
                    backRelated.Enqueue(backSingleUnit.SourceVertex);
                }
            }
            return backTree;
        }

        private static void AddEdges(List<InstructionNode> graph, List<LabeledVertex> labeledVertexes)
        {
            foreach (var instructionNode in graph)
            {
                LabeledVertex vertex = labeledVertexes[instructionNode.InstructionIndex];
                foreach (var dataFlowBackVertex in instructionNode.DataFlowBackRelated)
                {
                    vertex.BackEdges.Add(new LabeledEdge()
                    {
                        EdgeType = EdgeType.DataFlow,
                        Index = dataFlowBackVertex.ArgIndex,
                        SourceVertex = labeledVertexes[dataFlowBackVertex.Argument.InstructionIndex],
                        DestinationVertex = vertex
                    });
                }
                foreach (var programFlowBackVertex in instructionNode.ProgramFlowBackAffected)
                {
                    vertex.BackEdges.Add(new LabeledEdge()
                    {
                        EdgeType = EdgeType.ProgramFlowAffecting,
                        Index = programFlowBackVertex.ArgIndex,
                        SourceVertex = labeledVertexes[programFlowBackVertex.Argument.InstructionIndex],
                        DestinationVertex = vertex
                    });
                }
                foreach (var dataFlowBackVertex in instructionNode.DataFlowForwardRelated)
                {
                    vertex.ForwardEdges.Add(new LabeledEdge()
                    {
                        EdgeType = EdgeType.DataFlow,
                        Index = dataFlowBackVertex.ArgIndex,
                        SourceVertex = vertex,
                        DestinationVertex = labeledVertexes[dataFlowBackVertex.Argument.InstructionIndex]
                    });
                }
                foreach (var programFlowForwardVertex in instructionNode.ProgramFlowForwardAffecting)
                {
                    vertex.ForwardEdges.Add(new LabeledEdge()
                    {
                        EdgeType = EdgeType.ProgramFlowAffecting,
                        Index = programFlowForwardVertex.ArgIndex,
                        SourceVertex = vertex,
                        DestinationVertex = labeledVertexes[programFlowForwardVertex.Argument.InstructionIndex]
                    });
                }
                //foreach (var singleUnitBack in instructionNode.SingleUnitBackRelated)
                //{
                //    vertex.BackEdges.Add(new LabeledEdge()
                //    {
                //        EdgeType = EdgeType.SingleUnit,
                //        Index = 0,
                //        SourceVertex = labeledVertexes[singleUnitBack.InstructionIndex],
                //        DestinationVertex = vertex
                //    });
                //}
                //foreach (var singleUnitForward in instructionNode.SingleUnitForwardRelated)
                //{
                //    vertex.ForwardEdges.Add(new LabeledEdge()
                //    {
                //        EdgeType = EdgeType.SingleUnit,
                //        Index = 0,
                //        SourceVertex = vertex,
                //        DestinationVertex = labeledVertexes[singleUnitForward.InstructionIndex]
                //    });
                //}
            }
        }

        public static NodePairings GetSelfScore(List<InstructionNode> graph)
        {
            var labeledGraph = GetLabeled(graph);
            return GetSelfScore(labeledGraph);
        }
        public static NodePairings GetSelfScore(List<LabeledVertex> labeledGraph)
        {
            NodePairings nodePairings = new NodePairings(labeledGraph, labeledGraph);
            foreach (var node in labeledGraph)
            {
                int score = VertexScorePoints.VertexExactMatch + node.BackEdges.Concat(node.ForwardEdges).Sum(x => EdgeScorePoints.ExactMatch);
                nodePairings.Pairings[node].Add(new SingleNodePairing(node, score));
                nodePairings.Score += score;
            }
            return nodePairings;
        }
    }

    internal enum SharedSourceOrDest
    {
        Source,
        Dest
    }
}
