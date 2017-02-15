using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarityByMatching
{
    public static class VertexScorePoints
    {
        public const int VertexCodeFamilyMatch = 1;
        public const int VertexExactMatch = VertexCodeFamilyMatch + 2;
        public const int SingleToMultipleVertexMatchPenalty = 1;
    }
}
