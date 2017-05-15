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
            var edgePairings = new List<EdgeMatch>();
            var unmachedImageVertexEdges = new List<LabeledEdge>(imageVertexEdges);
            Random rnd = new Random();
            foreach (var sourceVertexEdge in sourceVertexEdges.OrderBy(x => rnd.Next()))
            {
                var pairingScores = new List<EdgeMatch>();
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
                    indexImportance = opCodeInfo.GetIndexImportance(sourceVertexEdge.DestinationVertex.Opcode);
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
                var relevantImageVertexEdges = unmachedImageVertexEdges.Where(predicate);
                relevantImageVertexEdges.ForEach(imageVertexEdge => pairingScores.Add(new EdgeMatch() { ImageGraphEdge = imageVertexEdge, SourceVertexEdge = sourceVertexEdge, Score = GetEdgeMatchScore(sourceVertexEdge, imageVertexEdge, sharedSourceOrDest, pairings, indexImportance) }));
                var winningMatchGroup = pairingScores.Where(x => x.Score > 0).GroupBy(x => x.Score).OrderByDescending(x => x.Key).FirstOrDefault();
                if (winningMatchGroup == null || !winningMatchGroup.Any())
                {
                    edgePairings.Add(new EdgeMatch() { SourceVertexEdge = sourceVertexEdge, ImageGraphEdge = null, Score = 0 });
                }
                else
                {
                    var winningMatch = winningMatchGroup.OrderBy(x => rnd.Next()).First();
                    unmachedImageVertexEdges.Remove(winningMatch.ImageGraphEdge);
                    winningMatch.Score = GetEdgeMatchScore(sourceVertexEdge, winningMatch.ImageGraphEdge, sharedSourceOrDest, pairings, indexImportance, false);
                    var scoreRelatedToMax = (double)winningMatch.Score / GetEdgeMatchScore(sourceVertexEdge, sourceVertexEdge, sharedSourceOrDest, pairings, IndexImportance.Important, false);
                    edgePairings.Add(winningMatch);
                    totalScore += winningMatch.Score;
                }
            }
            //totalScore -= unmachedImageVertexEdges.Count * EdgeScorePoints.ExactMatch;
            return totalScore;
        }

        public static int GetEdgeMatchScore(LabeledEdge sourceEdge, LabeledEdge imageEdge, SharedSourceOrDest sharingSourceOrDest, NodePairings pairings, IndexImportance indexImportance, bool usePastPairings = true)
        {
            int edgeMatchScore = 0;
            LabeledVertex sourceEdgeVertex;
            LabeledVertex imageEdgeVertex;
            if (sharingSourceOrDest == SharedSourceOrDest.Source)
            {
                sourceEdgeVertex = sourceEdge.DestinationVertex;
                imageEdgeVertex = imageEdge.DestinationVertex;
            }
            else
            {
                sourceEdgeVertex = sourceEdge.SourceVertex;
                imageEdgeVertex = imageEdge.SourceVertex;
            }

            if (imageEdge.Index == sourceEdge.Index || indexImportance == IndexImportance.NotImportant)
            {
                edgeMatchScore += EdgeScorePoints.IndexMatch;
            }
            if (usePastPairings)
            {
                lock (pairings)
                {
                    if (pairings.Pairings[imageEdgeVertex].Any(x => x.ImageGraphVertex == sourceEdgeVertex))
                    {
                        edgeMatchScore += EdgeScorePoints.TargetVertexArePaired;
                    }
                }
            }
            if (sourceEdgeVertex.Opcode == imageEdgeVertex.Opcode)
            {
                edgeMatchScore += EdgeScorePoints.TargetVertexCodeExactMatch;
            }
            else if (CodeGroups.AreSameGroup(sourceEdgeVertex.Opcode, imageEdgeVertex.Opcode))
            {
                edgeMatchScore += EdgeScorePoints.TargetVertexCodeFamilyMatch;
            }
            if (sourceEdge.EdgeType != EdgeType.ProgramFlowAffecting)
            {
                edgeMatchScore *= ImportantEdgeTypeMultiplier;
            }
            if (sourceEdge.SourceVertex.IsInReturnBackTree && sourceEdge.DestinationVertex.IsInReturnBackTree)
            {
                edgeMatchScore *= ImportantEdgeTypeMultiplier;
            }
            return edgeMatchScore;
        }
    }
}
