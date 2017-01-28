using DoppleTry2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarityByMatching
{
    class EdgeScorer
    {
        private static readonly CodeInfo opCodeInfo = new CodeInfo();
        private const int EdgeIndexMatchScore = 2;
        private const int DestinationVertexesAreMappedScore = 3;
        private const int DataFlowMutiplier = 5;
        private const int ProgramFlowAffectingMultiplier = 1;

        public static int ScoreEdges(List<LabeledEdge> firstEdges, List<LabeledEdge> secondEdges, Dictionary<LabeledVertex, List<LabeledVertex>> pairings, SharedSourceOrDest sharedSourceOrDest)
        {
            int totalScore = 0;
            var edgePairings = new Dictionary<LabeledEdge, LabeledEdge>();
            var unmachedSecondEdges = new List<LabeledEdge>(secondEdges);
            Random rnd = new Random();
            foreach (var firstEdge in firstEdges.OrderBy(x => rnd.Next()))
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
                Func<LabeledEdge, bool> predicate = x => x.EdgeType == firstEdge.EdgeType;
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
                var winningPairs = pairingScores.GroupBy(x => x.Value).OrderByDescending(x => x.Key).FirstOrDefault();
                if (winningPairs == null)
                {
                    edgePairings.Add(firstEdge, null);
                }
                else
                {
                    var winningPair = winningPairs.ElementAt(rnd.Next(0, winningPairs.Count()));
                    edgePairings.Add(firstEdge, winningPair.Key);
                    unmachedSecondEdges.Remove(winningPair.Key);
                    totalScore += winningPair.Value;
                }
            }
            return totalScore;
        }

        public static int GetEdgeMatchScore(Dictionary<LabeledVertex, List<LabeledVertex>> pairings, SharedSourceOrDest sourceOrDest, LabeledEdge firstEdge, LabeledVertex firstEdgeVertex, IndexImportance indexImportance, LabeledEdge secondEdge)
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
            if (firstEdge.EdgeType == EdgeType.DataFlow)
            {
                edgeMatchScore *= DataFlowMutiplier;
            }
            else if (firstEdge.EdgeType  == EdgeType.ProgramFlowAffecting)
            {
                edgeMatchScore *= ProgramFlowAffectingMultiplier;
            }
            return edgeMatchScore;
        }
    }
}
