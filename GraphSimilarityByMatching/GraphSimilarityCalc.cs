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
        private const int EdgeIndexMatchScore = 2;
        private const int DestinationVertexesAreMappedScore = 3;
        private static readonly CodeInfo opCodeInfo = new CodeInfo();
        private const int UnmachedVertexCost = 5;

        public static int GetDistance(List<InstructionNode> firstGraph, List<InstructionNode> secondGraph)
        {
            var pairings = new Dictionary<LabeledVertex, List<LabeledVertex>>();

            List<InstructionNode> biggerGraph;
            List<InstructionNode> smallerGraph;
            if (firstGraph.Count > secondGraph.Count)
            {
                biggerGraph = firstGraph;
                smallerGraph = secondGraph;
            }
            else
            {
                biggerGraph = secondGraph;
                smallerGraph = firstGraph;
            }
            List<LabeledVertex> biggerGraphLabeled = GetLabeled(biggerGraph);
            List<LabeledVertex> smallerGraphLabeled = GetLabeled(smallerGraph);

            smallerGraphLabeled.ForEach(x => pairings.Add(x, new List<LabeledVertex>()));

            int graphPairingScore = 0;
            foreach (var bigGraphVertex in biggerGraphLabeled)
            {
                var vertexPossiblePairings = new Dictionary<LabeledVertex, int>();
                var smallGraphCandidates = smallerGraphLabeled.Where(x => CodeGroups.AreSameGroup(x.Opcode, bigGraphVertex.Opcode)).ToList();
                foreach(var smallGraphCandidate in smallGraphCandidates)
                {
                    vertexPossiblePairings.Add( smallGraphCandidate, GetScore(smallGraphCandidate, bigGraphVertex, pairings));
                }
                var winningPair = vertexPossiblePairings.OrderByDescending(x => x.Value).FirstOrDefault();
                if (winningPair.Key != null)
                {
                    graphPairingScore += winningPair.Value;
                    pairings[winningPair.Key].Add(bigGraphVertex);
                }
                else
                {
                    graphPairingScore -= UnmachedVertexCost;
                }
            }
            return graphPairingScore;
        }

        private static int GetScore(LabeledVertex smallGraphVertex, LabeledVertex bigGraphVertex, Dictionary<LabeledVertex, List<LabeledVertex>> pairings)
        {
            int score = 1;
            if (smallGraphVertex.Opcode == bigGraphVertex.Opcode)
            {
                score += 1;
            }
            var backEdgeScore = ScoreEdges(smallGraphVertex.BackEdges, bigGraphVertex.BackEdges, pairings, SharedSourceOrDest.Dest);
            var forwardEdgeScore = ScoreEdges(smallGraphVertex.ForwardEdges, bigGraphVertex.ForwardEdges, pairings, SharedSourceOrDest.Source);
            score += backEdgeScore + forwardEdgeScore;
            score -= pairings[smallGraphVertex].Count;
            return score;
        }

        private static int ScoreEdges(List<LabeledEdge> firstEdges, List<LabeledEdge> secondEdges, Dictionary<LabeledVertex, List<LabeledVertex>> pairings, SharedSourceOrDest sharedSourceOrDest)
        {
            int totalScore = 0;
            var edgePairings = new Dictionary<LabeledEdge, LabeledEdge>();
            var unmachedSecondEdges = new List<LabeledEdge>(secondEdges);
            foreach(var firstEdge in firstEdges)
            {
                var pairingScores = new Dictionary<LabeledEdge, int>();
                LabeledVertex vertexToMatch;
                if (sharedSourceOrDest == SharedSourceOrDest.Source)
                {
                    vertexToMatch = firstEdge.DestinationVertex;
                }
                else
                {
                    vertexToMatch = firstEdge.SourceVertex;
                }
                Func<LabeledEdge,bool> predicate = x => x.EdgeType == firstEdge.EdgeType;
                IndexImportance indexImportance;
                if (firstEdge.EdgeType == EdgeType.ProgramFlowAffecting)
                {
                    indexImportance = IndexImportance.Important;
                }
                else
                {
                    indexImportance = opCodeInfo.GetIndexImportance(firstEdge.DestinationVertex.Opcode);
                }
                if (indexImportance == IndexImportance.Critical)
                {
                    predicate = x => x.EdgeType == firstEdge.EdgeType && x.Index == firstEdge.Index;
                }
                else
                {
                    predicate = x => x.EdgeType == firstEdge.EdgeType;
                }
                var relevantSecondEdges = unmachedSecondEdges.Where(predicate);
                relevantSecondEdges.ForEach(x => pairingScores.Add(x, GetEdgeMatchScore(pairings, sharedSourceOrDest, firstEdge, vertexToMatch, indexImportance, x)));
                KeyValuePair<LabeledEdge, int> winningPairing = pairingScores.OrderByDescending(x => x.Value).FirstOrDefault();
                if (winningPairing.Key == null)
                {
                    edgePairings.Add(firstEdge, null);
                }
                else
                {
                    edgePairings.Add(firstEdge, winningPairing.Key);
                    unmachedSecondEdges.Remove(winningPairing.Key);
                }
                totalScore += winningPairing.Value;
            }
            return totalScore;
        }

        private static int GetEdgeMatchScore(Dictionary<LabeledVertex, List<LabeledVertex>> pairings, SharedSourceOrDest sourceOrDest, LabeledEdge firstEdge, LabeledVertex firstEdgeVertex, IndexImportance indexImportance, LabeledEdge secondEdge)
        {
            int edgeMatchScore = 0;
          
            LabeledVertex secondEdgeVertex;
            if (sourceOrDest == SharedSourceOrDest.Source)
            {
                secondEdgeVertex = secondEdge.DestinationVertex;
            }
            else
            {
                secondEdgeVertex = secondEdge.SourceVertex;
            }

            if (indexImportance == IndexImportance.Important && secondEdge.Index == firstEdge.Index)
            {
                edgeMatchScore += EdgeIndexMatchScore;
            }
            if (pairings[firstEdgeVertex].Contains(secondEdgeVertex))
            {
                edgeMatchScore += DestinationVertexesAreMappedScore;
            }
            else if (firstEdgeVertex.Opcode == secondEdgeVertex.Opcode)
            {
                edgeMatchScore += 2;
            }
            else if (CodeGroups.AreSameGroup(firstEdgeVertex.Opcode, secondEdgeVertex.Opcode))
            {
                edgeMatchScore += 1;
            }
            return edgeMatchScore;
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
                labeledVertexes.Add(vertex);
            }
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
            }
            return labeledVertexes;
        }
    }

    internal enum SharedSourceOrDest
    {
        Source,
        Dest
    }
}
