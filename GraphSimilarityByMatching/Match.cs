using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarityByMatching
{
    public class VertexMatch
    {
        public LabeledVertex SourceGraphVertex { get; set; }
        public LabeledVertex ImageGraphVertex { get; set; }
        public double Score { get; set; }
        public double NormalizedScore { get; set; }
    }
}
