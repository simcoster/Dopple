using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace GraphSimilarityByMatching
{
    public class LabeledEdge
    {
        public LabeledVertex SourceVertex { get; set; }
        public LabeledVertex DestinationVertex { get; set; }
        public int Index { get; set; }
        public EdgeType EdgeType { get; set; }
    }
}