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
        const int MultiMatchPenalty = 1;

        public static double ScorePairings(Dictionary<LabeledVertex, List<LabeledVertex>> pairings)
        {
            int fullScore = 0;
            List<LabeledVertex> smallGraphVertexes = pairings.Keys.ToList();
            fullScore = smallGraphVertexes.Count * VertexFullCodeMatchPoints + EdgeMisMatchPenalty * (smallGraphVertexes.Sum(x => x.BackEdges.Count + x.ForwardEdges.Count));

            int matchScore = fullScore;
            foreach (var smallGraphVertex in pairings.Keys)
            {
                if (pairings[smallGraphVertex].Count > 1)
                {
                    matchScore -= MultiMatchPenalty * pairings[smallGraphVertex].Count - 1;
                }

                foreach (var bigGraphVertex in pairings[smallGraphVertex])
                {
                    if (bigGraphVertex.Opcode != smallGraphVertex.Opcode)
                    {
                        if (CodeGroups.AreSameGroup(bigGraphVertex.Opcode, smallGraphVertex.Opcode))
                        {
                            matchScore -= VertexCodeGroupMatchOnlyPenalty;
                        }
                        else
                        {
                            matchScore -= VertexFullCodeMatchPoints;
                        }
                    }

                    matchScore -= MatchEdgesPenalty(smallGraphVertex.BackEdges.ToList(), bigGraphVertex.BackEdges.ToList());
                    matchScore -= MatchEdgesPenalty(smallGraphVertex.ForwardEdges.ToList(), bigGraphVertex.ForwardEdges.ToList());
                }
            }
            return (double)matchScore / (double) fullScore;
        }

        private static int MatchEdgesPenalty(List<LabeledEdge> smallVertexEdges, List<LabeledEdge> bigVertexEdges)
        {
            int misMatchPenalty = 0;
            foreach (var smallGraphBackEdge in smallVertexEdges.ToArray())
            {
                var matchingSmallGraphEdge = bigVertexEdges.FirstOrDefault(x => x.SourceVertex.Opcode == smallGraphBackEdge.SourceVertex.Opcode &&
                                                                                                               x.Index == smallGraphBackEdge.Index &&
                                                                                                               x.EdgeType == smallGraphBackEdge.EdgeType);
                if (matchingSmallGraphEdge != null)
                {
                    bigVertexEdges.Remove(matchingSmallGraphEdge);
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
            var unmatchedCount = smallVertexEdges.Count + bigVertexEdges.Count;
            misMatchPenalty += unmatchedCount * EdgeMisMatchPenalty;
            return misMatchPenalty;
        }
    }
}
