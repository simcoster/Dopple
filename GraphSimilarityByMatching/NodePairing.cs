using GraphSimilarityByMatching;
using System.Collections.Generic;

namespace GraphSimilarityByMatching
{
    public class NodePairings
    {
        public NodePairings(List<LabeledVertex> firstGraph, List<LabeledVertex> secondGraph)
        {
            FirstGraph = firstGraph;
            SecondGraph = secondGraph;
            secondGraph.ForEach(x => Pairings.Add(x, new List<SingleNodePairing>()));
        }
        public List<LabeledVertex> FirstGraph { get; set; }
        public List<LabeledVertex> SecondGraph { get; set; }
        public Dictionary<LabeledVertex, List<SingleNodePairing>> Pairings { get; set; } = new Dictionary<LabeledVertex, List<SingleNodePairing>>();
        public int Score { get; set; } = 0;
    }
}