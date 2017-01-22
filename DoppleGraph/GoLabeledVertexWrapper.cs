using GraphSimilarityByMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleGraph
{
    public class GoLabeledVertexWrapper
    {
        public GoLabeledVertexWrapper(GoTextNodeHoverable node, LabeledVertex labledVertex)
        {
            Node = node;
            LabledVertex = labledVertex;
            Index = labledVertex.Index;
        }

        public GoTextNodeHoverable Node { get; private set; }
        public LabeledVertex LabledVertex { get; private set; }
        public int Index { get; set; }
        public float DisplayRow { get; set; }
        public int DisplayCol { get; set; }
        public List<GoLabeledVertexWrapper> LongestPath { get; set; } = new List<GoLabeledVertexWrapper>();
        public int DisplayColumn { get; internal set; }
    }
}
