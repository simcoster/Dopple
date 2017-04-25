using Dopple;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarityByMatching
{
    public static class VertexScorer
    {
        private const int ImportantCodeMultiplier = 50;
        private const int RetBackTreeMultiplier = 50;


        private static readonly List<Code> ImportantCodes = CodeGroups.LdElemCodes.Concat(CodeGroups.StElemCodes).Concat(CodeGroups.ArithmeticCodes).Concat(new[] { Code.Ret }).ToList();

        public static int GetScore(LabeledVertex sourceGraphVertex, LabeledVertex imageGraphVertex, NodePairings pairings)
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
            lock (pairings)
            {
                if (pairings.Pairings[imageGraphVertex].Count > 0)
                {
                    score -= VertexScorePoints.SingleToMultipleVertexMatchPenalty;
                }
            }
            if (ImportantCodes.Contains(sourceGraphVertex.Opcode))
            {
                score *= ImportantCodeMultiplier;
            }
            if (sourceGraphVertex.IsInReturnBackTree)
            {
                score *= RetBackTreeMultiplier;
            }
            return score;
        }

        public static double GetSelfScore(LabeledVertex labeledVertex)
        {
            int selfScore = VertexScorePoints.ExactMatch;
            object lockObject = new object();
            Parallel.ForEach(labeledVertex.BackEdges, (edge) =>
            {

                int score = EdgeScorer.GetEdgeMatchScore(edge, edge, SharedSourceOrDest.Dest, null, IndexImportance.Important, false);
                lock(lockObject)
                {
                    selfScore += score;
                }
            });
            Parallel.ForEach(labeledVertex.ForwardEdges, (edge) =>
            {
                int score = EdgeScorer.GetEdgeMatchScore(edge, edge, SharedSourceOrDest.Source, null, IndexImportance.Important, false);
                lock (lockObject)
                {
                    selfScore += score;
                }
            });
            if (ImportantCodes.Contains(labeledVertex.Opcode))
            {
                selfScore *= ImportantCodeMultiplier;
            }
            if (labeledVertex.IsInReturnBackTree)
            {
                selfScore *= RetBackTreeMultiplier;
            }
            return selfScore;
        }
    }
}
