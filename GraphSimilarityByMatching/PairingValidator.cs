using DoppleTry2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarityByMatching
{
    class PairingValidator
    {
        const int VertexFullCodeMatchPoints = 2;
        const int VertexCodeGroupMatchPoints = 1;
        const int EdgeRightVertexPoints = 1;
        const int EdgeRightIndexPoints = 1;
        const int EdgeFullMatchPoints = EdgeRightIndexPoints + EdgeRightVertexPoints;
        const int MultiMatchPenalty = 1;

        public static int ScorePairings( Dictionary<LabeledVertex, List<LabeledVertex>> pairings)
        {
            // Self pairing should recieve full score
            // Multi pairing should recieve penalty as to never contribute to the score, it is always a deduction, which is determined on how well is the pairing
            int fullScore = 0;
            List<LabeledVertex> smallGraphVertexes = pairings.Keys.ToList();
            fullScore = smallGraphVertexes.Count * VertexFullCodeMatchPoints + EdgeFullMatchPoints * (smallGraphVertexes.Sum(x => x.BackEdges.Count + x.ForwardEdges.Count));

            int matchScore = fullScore;
            foreach (var smallGraphVertex in pairings.Keys)
            {
                if (pairings[smallGraphVertex].Count > 1)
                {
                    matchScore -= MultiMatchPenalty * pairings[smallGraphVertex].Count - 1;
                }


                foreach (var bigGraphVertex in pairings[smallGraphVertex])
                {
                    if (bigGraphVertex.Opcode == smallGraphVertex.Opcode)
                    {
                        matchScore += VertexFullCodeMatchPoints;
                    }
                    else if (CodeGroups.AreSameGroup(bigGraphVertex.Opcode,smallGraphVertex.Opcode))
                    {
                        matchScore += VertexCodeGroupMatchPoints;
                    }
                    fullScore += VertexFullCodeMatchPoints;

                    foreach (var smallGraphBackEdge in smallGraphVertex.BackEdges)
                    {
                        var matchingSmallGraphEdge = bigGraphVertex.BackEdges.FirstOrDefault(x => x.SourceVertex.Opcode == smallGraphBackEdge.SourceVertex.Opcode && 
                                                                                                                       x.Index == smallGraphBackEdge.Index && 
                                                                                                                       x.EdgeType == smallGraphBackEdge.EdgeType);
                        if (matchingSmallGraphEdge != null)
                        {
                            matchScore += EdgeRightVertexPoints + EdgeRightIndexPoints;
                            bigGraphVertex.BackEdges.Remove(matchingSmallGraphEdge);
                            smallGraphVertex.BackEdges.Remove(smallGraphBackEdge);
                        }
                    }
                    foreach (var smallGraphBackEdge in smallGraphVertex.BackEdges)
                    {
                        var matchingSmallGraphEdge = bigGraphVertex.BackEdges.FirstOrDefault(x => x.SourceVertex.Opcode == smallGraphBackEdge.SourceVertex.Opcode &&
                                                                                                                       x.EdgeType == smallGraphBackEdge.EdgeType);
                        if (matchingSmallGraphEdge != null)
                        {
                            matchScore += EdgeRightVertexPoints;
                            bigGraphVertex.BackEdges.Remove(matchingSmallGraphEdge);
                            smallGraphVertex.BackEdges.Remove(smallGraphBackEdge);
                        }
                    }

                }
            }
            return 0;
        }
    }
}
