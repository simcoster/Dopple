using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarityByMatching
{
    public static class VertexScorePoints
    {
        public const int CodeFamilyMatch = 1;
        public const int CodeMatch = 2;
        public const int OperandMatch = 1;
        public const int ExactMatch = CodeMatch + OperandMatch;
        public const int SingleToMultipleVertexMatchPenalty = 10;
    }
}
