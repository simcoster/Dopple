using GraphSimilarityByMatching;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GraphSimilarityByMatching
{
    public class NodePairings
    {
        public NodePairings(List<LabeledVertex> imageGraph, List<LabeledVertex> sourceGraph)
        {
            imageGraph = ImageGraph;
            SourceGraph = sourceGraph;
            SourceGraph.ForEach(x => Pairings.Add(x, new ConcurrentBag<SingleNodePairing>()));
        }
        public List<LabeledVertex> ImageGraph { get; set; }
        public List<LabeledVertex> SourceGraph { get; set; }
        public Dictionary<LabeledVertex, ConcurrentBag<SingleNodePairing>> Pairings { get; set; } = new Dictionary<LabeledVertex, ConcurrentBag<SingleNodePairing>>();
        public double TotalScore { get; set; } = 0;
    }
}