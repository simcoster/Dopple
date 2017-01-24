using GraphSimilarityByMatching;
using System.Collections.Generic;

namespace GraphSimilarityByMatching
{
    public class NodePairing
    {
        public List<LabeledVertex> BigGraph { get; set; }
        public List<LabeledVertex> SmallGraph { get; set; }
        public Dictionary<LabeledVertex, List<LabeledVertex>> Pairings { get; set; } = new Dictionary<LabeledVertex, List<LabeledVertex>>();
        public List<PairingPenalty> penalties = new List<PairingPenalty>();
    }
}