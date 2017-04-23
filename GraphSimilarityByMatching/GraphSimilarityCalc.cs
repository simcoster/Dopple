using Dopple;
using Dopple.InstructionNodes;
using Mono.Cecil.Cil;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarityByMatching
{
    public class GraphSimilarityCalc
    {
        private const int ImportantCodeMultiplier = 50;
        private static readonly CodeInfo opCodeInfo = new CodeInfo();
        private static readonly List<Code> ImportantCodes = CodeGroups.LdElemCodes.Concat(CodeGroups.StElemCodes).Concat(CodeGroups.ArithmeticCodes).Concat(new[] { Code.Ret }).ToList();


        public static NodePairings GetDistance(List<InstructionNode> sourceGraph, List<InstructionNode> imageGraph)
        {
            List<LabeledVertex> sourceGraphLabeled = GetLabeled(sourceGraph);
            List<LabeledVertex> imageGraphLabeled = GetLabeled(imageGraph);
            NodePairings bestMatch = GetPairings(sourceGraphLabeled, imageGraphLabeled);
            //TODO change back to 10
            Parallel.For(1, 2, (i) =>
            {
                NodePairings pairing = GetPairings(sourceGraphLabeled, imageGraphLabeled);
                if (pairing.TotalScore > bestMatch.TotalScore)
                {
                    bestMatch = pairing;
                }
            });
            return bestMatch ;
        }

        private static NodePairings GetPairings(List<LabeledVertex> sourceGraphLabeled, List<LabeledVertex> imageGraphLabeled)
        {
            NodePairings nodePairings = new NodePairings(sourceGraphLabeled, imageGraphLabeled);

            Random rnd = new Random();
            var sourceGraphGrouped= CodeGroups.CodeGroupLists.AsParallel().ToDictionary(x =>x , x=> sourceGraphLabeled.Where(y => x.Contains(y.Opcode)).ToList());
            var imageGraphGrouped = CodeGroups.CodeGroupLists.AsParallel().ToDictionary(x => x, x => imageGraphLabeled.Where(y => x.Contains(y.Opcode)).ToList());

            Parallel.ForEach(sourceGraphGrouped.Keys, (codeGroup) =>
            {
                //not gonna lock for now
                foreach(var sourceNode in sourceGraphGrouped[codeGroup])
                {
                    var vertexPossiblePairings = new ConcurrentBag<Tuple<LabeledVertex, int>>();
                    Parallel.ForEach(imageGraphGrouped[codeGroup].OrderBy(x => rnd.Next()).ToList(), (imageGraphCandidate) =>
                   {
                       vertexPossiblePairings.Add(new Tuple<LabeledVertex, int>(imageGraphCandidate, GetScore(sourceNode, imageGraphCandidate, nodePairings)));
                   });
                }
            });
            return nodePairings;
            Parallel.ForEach(sourceGraphLabeled.OrderBy(x => rnd.Next()).ToList(), (sourceGraphVertex) =>
            {
                
                var winningPairs = vertexPossiblePairings.Where(x => x.Value > 0).GroupBy(x => x.Value).OrderByDescending(x => x.Key).sourceOrDefault();
                if (winningPairs != null)
                {
                    KeyValuePair<LabeledVertex, int> winningPair = winningPairs.ElementAt(rnd.Next(0, winningPairs.Count()));
                    int winningPairScore = winningPair.Value;
                    if (ImportantCodes.Contains(sourceGraphVertex.Opcode))
                    {
                        winningPairScore *= ImportantCodeMultiplier;
                    }
                    nodePairings.TotalScore += winningPairScore;
                    lock (nodePairings.Pairings)
                    {
                        nodePairings.Pairings[winningPair.Key].Add(new SingleNodePairing(sourceGraphVertex, winningPairScore));
                    }
                }
                else
                {
                    var selfPairing = GetSelfPairingScore(sourceGraphVertex);
                    nodePairings.TotalScore -= selfPairing;
                }
            });
            return nodePairings;
        }

        private static int GetScore(LabeledVertex sourceGraphVertex, LabeledVertex imageGraphVertex, NodePairings pairings)
        {
            int score = 0;
            if (sourceGraphVertex.Opcode == imageGraphVertex.Opcode)
            {
                score += VertexScorePoints.CodeMatch;
            }
            else
            {
                score += VertexScorePoints.CodeFamilyMatch;
            }
            if (sourceGraphVertex.Operand == imageGraphVertex.Operand)
            {
                score += VertexScorePoints.OperandMatch;
            }
            var backEdgeScore = EdgeScorer.ScoreEdges(sourceGraphVertex.BackEdges, imageGraphVertex.BackEdges, pairings, SharedSourceOrDest.Dest);
            var forwardEdgeScore = EdgeScorer.ScoreEdges(sourceGraphVertex.ForwardEdges, imageGraphVertex.ForwardEdges, pairings, SharedSourceOrDest.Source);
            score += backEdgeScore + forwardEdgeScore;
            lock(pairings)
            {
                if (pairings.Pairings[imageGraphVertex].Count > 0)
                {
                    score -= VertexScorePoints.SingleToMultipleVertexMatchPenalty;
                }
            }
            return score;
        }
        private static List<LabeledVertex> GetLabeled(List<InstructionNode> graph)
        {
            var labeledVertexes = graph.AsParallel().Select(x => new LabeledVertex()
            {
                Opcode = x.Instruction.OpCode.Code,
                Operand = x.Instruction.Operand,
                Index = x.InstructionIndex,
                Method = x.Method,
            }).ToList();
            Parallel.ForEach(graph, (node) =>
            {
                AddEdges(node, labeledVertexes);
            });
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

        private static void AddEdges(InstructionNode node, List<LabeledVertex> labeledVertexes)
        {

            LabeledVertex vertex = labeledVertexes[node.InstructionIndex];
            foreach (var dataFlowBackVertex in node.DataFlowBackRelated)
            {
                vertex.BackEdges.Add(new LabeledEdge()
                {
                    EdgeType = EdgeType.DataFlow,
                    Index = dataFlowBackVertex.ArgIndex,
                    SourceVertex = labeledVertexes[dataFlowBackVertex.Argument.InstructionIndex],
                    DestinationVertex = vertex
                });
            }
            foreach (var branch in node.BranchProperties.Branches)
            {
                vertex.BackEdges.Add(new LabeledEdge()
                {
                    EdgeType = EdgeType.ProgramFlowAffecting,
                    Index = (int) branch.PairedBranchesIndex,
                    SourceVertex = labeledVertexes[branch.OriginatingNode.InstructionIndex],
                    DestinationVertex = vertex
                });
            }
            foreach (var dataFlowBackVertex in node.DataFlowForwardRelated)
            {
                vertex.ForwardEdges.Add(new LabeledEdge()
                {
                    EdgeType = EdgeType.DataFlow,
                    Index = dataFlowBackVertex.ArgIndex,
                    SourceVertex = vertex,
                    DestinationVertex = labeledVertexes[dataFlowBackVertex.Argument.InstructionIndex]
                });
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
                int score = GetSelfPairingScore(node);
               
                nodePairings.Pairings[node].Add(new SingleNodePairing(node, score));
                nodePairings.TotalScore += score;
            }
            return nodePairings;
        }

        private static int GetSelfPairingScore(LabeledVertex node)
        {
            int score = VertexScorePoints.ExactMatch;
            score += node.BackEdges.Concat(node.ForwardEdges).Where(x => x.EdgeType == EdgeType.ProgramFlowAffecting).Sum(x => EdgeScorePoints.ExactMatch);
            score += node.BackEdges.Concat(node.ForwardEdges).Where(x => x.EdgeType != EdgeType.ProgramFlowAffecting).Sum(x => EdgeScorePoints.ExactMatch * 3);
            if (ImportantCodes.Contains(node.Opcode))
            {
                score *= ImportantCodeMultiplier;
            }
            return score;
        }
    }

    internal enum SharedSourceOrDest
    {
        Source,
        Dest
    }
}
