using GraphSimilarityByMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarityByMatching
{
    public class SmallBigLinkEdge
    {
        public LabeledVertex ImageVertex { get; set; }
        public LabeledVertex SourceVertex { get; set; }
        public double Score { get; set; }
    }
}
