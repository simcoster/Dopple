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
        public List<LabledEdge> BackEdges { get; set; }
        public List<LabledEdge> ForwardEdges { get; set; }
    }
}
