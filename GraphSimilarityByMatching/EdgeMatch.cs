namespace GraphSimilarityByMatching
{
    internal class EdgeMatch
    {
        public int Score { get; internal set; }
        public LabeledEdge ImageGraphEdge { get; internal set; }
        public LabeledEdge  SourceVertexEdge { get; internal set; }
    }
}