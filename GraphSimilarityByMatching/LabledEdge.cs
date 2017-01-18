using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace GraphSimilarityByMatching
{
    public class LabeledEdge
    {
        public Code SourceVertexOpcode { get; set; }
        public Code DestVertexOpcode { get; set; }
        public int Index { get; set; }
        public EdgeType EdgeType { get; set; }
    }
}