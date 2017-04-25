using GraphSimilarityByMatching;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GraphSimilarityByMatching
{
    public class NodePairings
    {
        public NodePairings(List<LabeledVertex> imageGraph, List<LabeledVertex> sourceGraph)
        {
            ImageGraph = imageGraph;
            SourceGraph = sourceGraph;
            ImageGraph.ForEach(x => Pairings.Add(x, new ConcurrentBag<VertexMatch>()));
        }
        public List<LabeledVertex> ImageGraph { get; set; }
        public List<LabeledVertex> SourceGraph { get; set; }
        public Dictionary<LabeledVertex, ConcurrentBag<VertexMatch>> Pairings { get; set; } = new Dictionary<LabeledVertex, ConcurrentBag<VertexMatch>>();
        public double TotalScore { get; set; } = 0;
    }
}