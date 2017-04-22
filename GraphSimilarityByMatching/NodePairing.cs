using GraphSimilarityByMatching;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GraphSimilarityByMatching
{
    public class NodePairings
    {
        public NodePairings(List<LabeledVertex> firstGraph, List<LabeledVertex> secondGraph)
        {
            FirstGraph = firstGraph;
            SecondGraph = secondGraph;
            secondGraph.ForEach(x => Pairings.Add(x, new ConcurrentBag<SingleNodePairing>()));
        }
        public List<LabeledVertex> FirstGraph { get; set; }
        public List<LabeledVertex> SecondGraph { get; set; }
        public Dictionary<LabeledVertex, ConcurrentBag<SingleNodePairing>> Pairings { get; set; } = new Dictionary<LabeledVertex, ConcurrentBag<SingleNodePairing>>();
        public double TotalScore { get; set; } = 0;
    }
}