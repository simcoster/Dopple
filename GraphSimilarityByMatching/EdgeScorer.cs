using Dopple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarityByMatching
{
    class EdgeScorer
    {
        public const int ImportantEdgeTypeMultiplier = 3;
        private static readonly CodeInfo opCodeInfo = new CodeInfo();

        public static int ScoreEdges(List<LabeledEdge> sourceVertexEdges, List<LabeledEdge> imageVertexEdges, NodePairings pairings, SharedSourceOrDest sharedSourceOrDest)
        {
            int totalScore = 0;
            var edgePairings = new Dictionary<LabeledEdge, LabeledEdge>();
            var unmachedImageVertexEdges = new List<LabeledEdge>(imageVertexEdges);
            Random rnd = new Random();
            foreach (var sourceVertexEdge in sourceVertexEdges.OrderBy(x => rnd.Next()))
            {
                var pairingScores = new List<Tuple<LabeledEdge, int>>();
                LabeledVertex vertexToMatch;
                if (sharedSourceOrDest == SharedSourceOrDest.Source)
                {
                    vertexToMatch = sourceVertexEdge.DestinationVertex;
                }
                else
                {
                    vertexToMatch = sourceVertexEdge.SourceVertex;
                }
                Func<LabeledEdge, bool> baseCondition = x => x.EdgeType == sourceVertexEdge.EdgeType;
                IndexImportance indexImportance;
                if (sourceVertexEdge.EdgeType == EdgeType.ProgramFlowAffecting)
                {
                    indexImportance = IndexImportance.Important;
                }
                else
                {
                    indexImportance = opCodeInfo.GetIndexImportance(vertexToMatch.Opcode);
                }
                Func<LabeledEdge, bool> predicate;
                if (indexImportance == IndexImportance.Critical)
                {
                    predicate = x => baseCondition(x) && x.Index == sourceVertexEdge.Index;
                }
                else
                {
                    predicate = x => baseCondition(x);
                }
                var relevantSecondEdges = unmachedImageVertexEdges.Where(predicate);
                relevantSecondEdges.ForEach(secondEdge => pairingScores.Add(new Tuple<LabeledEdge, int>(secondEdge, GetEdgeMatchScore(sourceVertexEdge, secondEdge, sharedSourceOrDest, pairings, indexImportance))));
                var possiblePairings = pairingScores.Where(x => x.Item2 > 0).GroupBy(x => x.Item1).OrderByDescending(x => x.Key).FirstOrDefault();
                if (possiblePairings == null)
                {
                    edgePairings.Add(sourceVertexEdge, null);
                }
                else
                {
                    Tuple<LabeledEdge,int> winningPair = possiblePairings.ElementAt(rnd.Next(0, possiblePairings.Count()));
                    edgePairings.Add(sourceVertexEdge, winningPair.Item1);
                    unmachedImageVertexEdges.Remove(winningPair.Item1);
                    var winningMatchScore = GetEdgeMatchScore(sourceVertexEdge, winningPair.Item1, sharedSourceOrDest, pairings, indexImportance, false);
                    totalScore += winningMatchScore;
                }
            }
            totalScore -= unmachedImageVertexEdges.Count * EdgeScorePoints.ExactMatch;
            return totalScore;
        }

        public static int GetEdgeMatchScore(LabeledEdge firstEdge, LabeledEdge secondEdge, SharedSourceOrDest sharingSourceOrDest, NodePairings pairings, IndexImportance indexImportance, bool usePastPairings = true)
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

            if (secondEdge.Index == firstEdge.Index || indexImportance == IndexImportance.NotImportant)
            {
                edgeMatchScore += EdgeScorePoints.IndexMatch;
            }
            lock(pairings)
            {
                if (usePastPairings && pairings.Pairings[secondEdgeVertex].Any(x => x.PairedVertex == firstEdgeVertex))
                {
                    edgeMatchScore += EdgeScorePoints.TargetVertexArePaired;
                }
            }
            if (firstEdgeVertex.Opcode == secondEdgeVertex.Opcode)
            {
                edgeMatchScore += EdgeScorePoints.TargetVertexCodeExactMatch;
            }
            else if (CodeGroups.AreSameGroup(firstEdgeVertex.Opcode, secondEdgeVertex.Opcode))
            {
                edgeMatchScore += EdgeScorePoints.TargetVertexCodeFamilyMatch;
            }
            if (firstEdge.EdgeType != EdgeType.ProgramFlowAffecting)
            {
                edgeMatchScore *= ImportantEdgeTypeMultiplier;
            }
            return edgeMatchScore;
        }
    }
}
