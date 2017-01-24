using DoppleTry2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarityByMatching
{
    public class PairingValidator
    {
        const int VertexFullCodeMatchPoints = 2;
        const int VertexCodeGroupMatchOnlyPenalty = 1;
        const int EdgeWrongVertexCodePenalty = 1;
        const int EdgeWrongIndexPenalty = 1;
        const int EdgeMisMatchPenalty = EdgeWrongIndexPenalty + EdgeWrongVertexCodePenalty;
        const double MultiMatchPenalty = 0.5;
        private static List<LabeledVertex> UnmatchedBigGraph;
        private static List<LabeledVertex> UnmatchedSmallGraph;

        public static double ScorePairings(NodePairing nodePairings)
        {
            List<PairingPenalty> pairingPenalties = new List<PairingPenalty>();
            var pairings = nodePairings.Pairings;
            UnmatchedBigGraph = nodePairings.BigGraph.Except(pairings.Values.SelectMany(x => x)).ToList();
            UnmatchedSmallGraph = nodePairings.Pairings.Where(x => x.Value.Count ==0).Select(x => x.Key).ToList();
            double fullScore = 0;
            List<LabeledVertex> smallGraphVertexes = pairings.Keys.ToList();
            fullScore = nodePairings.BigGraph.Count * VertexFullCodeMatchPoints + EdgeMisMatchPenalty * (nodePairings.BigGraph.Sum(x => x.BackEdges.Count + x.ForwardEdges.Count));

            double matchScore = fullScore;
            foreach (var pairing in pairings)
            {
                pairing.Value.ForEach(x => pairingPenalties.Add(new PairingPenalty(x, pairing.Key)));
                var smallGraphVertex = pairing.Key;
                if (pairing.Value.Count > 1)
                {
                    var multiMatchPenaltyPropertional = MultiMatchPenalty * pairing.Value.Count - 1;
                    matchScore -= multiMatchPenaltyPropertional;
                    pairing.Value.ForEach(x => pairingPenalties.First(y => y.BigGraphVertex == x).Penalty += multiMatchPenaltyPropertional / pairing.Value.Count);
                }

                foreach (var bigGraphVertex in pairing.Value)
                {
                    if (bigGraphVertex.Opcode != smallGraphVertex.Opcode)
                    {
                        if (CodeGroups.AreSameGroup(bigGraphVertex.Opcode, smallGraphVertex.Opcode))
                        {
                            matchScore -= VertexCodeGroupMatchOnlyPenalty;
                            pairingPenalties.First(x => x.BigGraphVertex == bigGraphVertex).Penalty += VertexCodeGroupMatchOnlyPenalty;
                        }
                        else
                        {
                            matchScore -= VertexFullCodeMatchPoints;
                            pairingPenalties.First(x => x.BigGraphVertex == bigGraphVertex).Penalty += VertexFullCodeMatchPoints;
                        }
                    }
                    var backEdgesPenalty = MatchEdgesPenalty(smallGraphVertex.BackEdges.ToList(), bigGraphVertex.BackEdges.ToList(), nodePairings);
                    var forwardEdgesPenalty = MatchEdgesPenalty(smallGraphVertex.ForwardEdges.ToList(), bigGraphVertex.ForwardEdges.ToList(), nodePairings);
                    matchScore -= backEdgesPenalty;
                    matchScore -= forwardEdgesPenalty;
                    pairingPenalties.First(x => x.BigGraphVertex == bigGraphVertex).Penalty += backEdgesPenalty + forwardEdgesPenalty;
                }
            }
            nodePairings.penalties = pairingPenalties;
            return (double)matchScore / (double) fullScore;
        }

        private static int MatchEdgesPenalty(List<LabeledEdge> smallVertexEdges, List<LabeledEdge> bigVertexEdges, NodePairing pairing)
        {
            int misMatchPenalty = 0;
            foreach (var smallGraphBackEdge in smallVertexEdges.ToArray())
            {
                var exactMatchBigGraphEdge = bigVertexEdges.FirstOrDefault(x => x.SourceVertex.Opcode == smallGraphBackEdge.SourceVertex.Opcode &&
                                                                                                               x.Index == smallGraphBackEdge.Index &&
                                                                                                               x.EdgeType == smallGraphBackEdge.EdgeType);
                if (exactMatchBigGraphEdge != null)
                {
                    bigVertexEdges.Remove(exactMatchBigGraphEdge);
                    smallVertexEdges.Remove(smallGraphBackEdge);
                }
            }
            foreach (var smallGraphBackEdge in smallVertexEdges.ToArray())
            {
                var matchingSmallGraphEdge = bigVertexEdges.FirstOrDefault(x => x.SourceVertex.Opcode == smallGraphBackEdge.SourceVertex.Opcode &&
                                                                                                               x.EdgeType == smallGraphBackEdge.EdgeType);
                if (matchingSmallGraphEdge != null)
                {
                    misMatchPenalty += EdgeWrongIndexPenalty;
                    bigVertexEdges.Remove(matchingSmallGraphEdge);
                    smallVertexEdges.Remove(smallGraphBackEdge);
                }
            }
            foreach (var smallGraphBackEdge in smallVertexEdges.ToArray())
            {
                var matchingSmallGraphEdge = bigVertexEdges.FirstOrDefault(x => CodeGroups.AreSameGroup(x.SourceVertex.Opcode,smallGraphBackEdge.SourceVertex.Opcode)
                                                                                                               && x.Index == smallGraphBackEdge.Index
                                                                                                               && x.EdgeType == smallGraphBackEdge.EdgeType);
                if (matchingSmallGraphEdge != null)
                {
                    misMatchPenalty += EdgeWrongVertexCodePenalty;
                    bigVertexEdges.Remove(matchingSmallGraphEdge);
                    smallVertexEdges.Remove(smallGraphBackEdge);
                }
            }
            var allUmatched = UnmatchedBigGraph.Concat(UnmatchedSmallGraph).ToList();
            var unmatchedCount = smallVertexEdges.Concat(bigVertexEdges).Count(x => !allUmatched.Contains(x.DestinationVertex) && !allUmatched.Contains(x.SourceVertex));
            misMatchPenalty += unmatchedCount * EdgeMisMatchPenalty;
            return misMatchPenalty;
        }
    }
}
