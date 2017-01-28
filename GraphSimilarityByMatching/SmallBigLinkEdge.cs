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
        public LabeledVertex SmallGraphVertex { get; set; }
        public LabeledVertex BigGraphVertex { get; set; }
        public double Score { get; set; }
    }
}
