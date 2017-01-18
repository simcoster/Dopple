using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarityByMatching
{
    class LabeledVertex
    {
        public Code Opcode { get; set; }
        public object Operand { get; set; }
        public List<LabeledEdge> BackEdges { get; set; } = new List<LabeledEdge>();
        public List<LabeledEdge> ForwardEdges { get; set; } = new List<LabeledEdge>();
    }
}
