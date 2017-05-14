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
        private const int ImportantCodeMultiplier = 5;
        private const int RetBackTreeMultiplier = 5;


        private static readonly Code[] ImportantCodes = new Code[] { }.Concat(CodeGroups.StoreFieldCodes).Concat(CodeGroups.StElemCodes).Concat(CodeGroups.ArithmeticCodes).Concat(new[] { Code.Ret }).ToArray();

        public static double GetScore(LabeledVertex sourceGraphVertex, LabeledVertex imageGraphVertex, NodePairings pairings)
        {
            double score = 0;
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
                    //score -= VertexScorePoints.SingleToMultipleVertexMatchPenalty;
                    score *= 0.9;
                }
            }
            if (ImportantCodes.Contains(sourceGraphVertex.Opcode))
            {
                score *= ImportantCodeMultiplier;
            }
            //if (sourceGraphVertex.IsInReturnBackTree)
            //{
            //    score *= RetBackTreeMultiplier;
            //}
            var scoreToDouble = score / GetSelfScore(sourceGraphVertex); 
            return score;
        }

        public static double GetSelfScore(LabeledVertex labeledVertex)
        {
            int selfScore = VertexScorePoints.ExactMatch;
            object lockObject = new object();
            //Parallel.ForEach(labeledVertex.BackEdges, (edge) =>
            foreach(var edge in labeledVertex.BackEdges)
            {

                int score = EdgeScorer.GetEdgeMatchScore(edge, edge, SharedSourceOrDest.Dest, null, IndexImportance.Important, false);
                lock(lockObject)
                {
                    selfScore += score;
                }
            }
            //Parallel.ForEach(labeledVertex.ForwardEdges, (edge) =>
            foreach (var edge in labeledVertex.ForwardEdges)
            {
                int score = EdgeScorer.GetEdgeMatchScore(edge, edge, SharedSourceOrDest.Source, null, IndexImportance.Important, false);
                lock (lockObject)
                {
                    selfScore += score;
                }
            }
            if (ImportantCodes.Contains(labeledVertex.Opcode))
            {
                selfScore *= ImportantCodeMultiplier;
            }
            return selfScore;
        }
    }
}
