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

        public static int ScoreEdges(List<LabeledEdge> firstEdges, List<LabeledEdge> secondEdges, NodePairings pairings, SharedSourceOrDest sharedSourceOrDest)
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
                relevantSecondEdges.ForEach(secondEdge => pairingScores.Add(secondEdge, GetEdgeMatchScore(firstEdge, secondEdge, sharedSourceOrDest, pairings)));
                var winningPairs = pairingScores.GroupBy(x => x.Value).OrderByDescending(x => x.Key).FirstOrDefault();
                if (winningPairs == null)
                {
                    edgePairings.Add(firstEdge, null);
                }
                else
                {
                    KeyValuePair<LabeledEdge,int> winningPair = winningPairs.ElementAt(rnd.Next(0, winningPairs.Count()));
                    edgePairings.Add(firstEdge, winningPair.Key);
                    unmachedSecondEdges.Remove(winningPair.Key);
                    var winningMatchScore = GetEdgeMatchScore(firstEdge, winningPair.Key, sharedSourceOrDest, pairings, false);
                    totalScore += winningMatchScore;
                }
            }
            totalScore -= unmachedSecondEdges.Count * EdgeScorePoints.ExactMatch;
            return totalScore;
        }

        public static int GetEdgeMatchScore(LabeledEdge firstEdge, LabeledEdge secondEdge, SharedSourceOrDest sharingSourceOrDest, NodePairings pairings, bool usePastPairings = true)
        {
            int edgeMatchScore = 0;
            LabeledVertex firstEdgeVertex;
            LabeledVertex secondEdgeVertex;
            if (sharingSourceOrDest == SharedSourceOrDest.Source)
            {
                firstEdgeVertex = firstEdge.DestinationVertex;
                secondEdgeVertex = secondEdge.DestinationVertex;
            }
            else
            {
                firstEdgeVertex = firstEdge.SourceVertex;
                secondEdgeVertex = secondEdge.SourceVertex;
            }

            if (secondEdge.Index == firstEdge.Index)
            {
                edgeMatchScore += EdgeScorePoints.IndexMatch;
            }
            if (usePastPairings && pairings.Pairings[secondEdgeVertex].Any(x => x.PairedVertex ==firstEdgeVertex))
            {
                edgeMatchScore += EdgeScorePoints.TargetVertexArePaired;
            }
            if (firstEdgeVertex.Opcode == secondEdgeVertex.Opcode)
            {
                edgeMatchScore += EdgeScorePoints.TargetVertexCodeExactMatch;
            }
            else if (CodeGroups.AreSameGroup(firstEdgeVertex.Opcode, secondEdgeVertex.Opcode))
            {
                edgeMatchScore += EdgeScorePoints.TargetVertexCodeFamilyMatch;
            }
            return edgeMatchScore;
        }
    }
}
