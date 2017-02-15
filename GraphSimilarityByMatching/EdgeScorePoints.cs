using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarityByMatching
{
    public static class EdgeScorePoints
    {
        public const int IndexMatch = 1;
        public const int TargetVertexCodeFamilyMatch = VertexScorePoints.CodeFamilyMatch;
        public const int TargetVertexCodeExactMatch = VertexScorePoints.ExactMatch;
        public const int TargetVertexArePaired =1;
        public const int ExactMatch = IndexMatch  + TargetVertexCodeExactMatch;
    }
}
