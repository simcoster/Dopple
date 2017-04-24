using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarityByMatching
{
    public class SingleNodePairing
    {
        public SingleNodePairing(LabeledVertex pairedVertex, double pairingScore)
        {
            PairedVertex = pairedVertex;
            PairingScore = pairingScore;
        }
        public LabeledVertex PairedVertex { get; set; }
        public double PairingScore { get; set; }
    }
}
