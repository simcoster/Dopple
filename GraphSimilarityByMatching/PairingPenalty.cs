namespace GraphSimilarityByMatching
{
    public class PairingPenalty
    {
        public PairingPenalty(LabeledVertex bigGraphVertex , LabeledVertex smallGraphVertex)
        {
            BigGraphVertex = bigGraphVertex;
            SmallGraphVertex = smallGraphVertex;
        }
        public LabeledVertex BigGraphVertex { get; private set; }
        public LabeledVertex SmallGraphVertex { get; private set; }
        public double Penalty { get; set; } = 0;
    }
}