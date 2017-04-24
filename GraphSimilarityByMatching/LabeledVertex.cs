using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarityByMatching
{
    public class LabeledVertex
    {
        public LabeledVertex()
        { }
        public Code Opcode { get; set; }
        public object Operand { get; set; }
        public int Index { get; set; }
        public List<LabeledEdge> BackEdges { get; set; } = new List<LabeledEdge>();
        public List<LabeledEdge> ForwardEdges { get; set; } = new List<LabeledEdge>();
        public List<SmallBigLinkEdge> PairingEdges { get; set; } = new List<SmallBigLinkEdge>(); 
        public MethodDefinition Method { get; set; }
        public Guid Guid = Guid.NewGuid();
        public bool IsInReturnBackTree { get; set; } = false;
    }
    public class CompoundedLabeledVertex : LabeledVertex
    {
        private LabeledVertex FirstMostVertex;
        public CompoundedLabeledVertex(LabeledVertex firstMostVertex)
        {
            FirstMostVertex = firstMostVertex;
            Opcode = firstMostVertex.Opcode;
            Operand = firstMostVertex.Operand;
            Index = firstMostVertex.Index;
        }
        public void AddBackInnerVertexes(IEnumerable<LabeledVertex> innerVertexes)
        {
            var groupedVertexes = innerVertexes.Concat(new[] { FirstMostVertex });
            foreach (var innerVertex in groupedVertexes)
            {
                innerVertex.BackEdges.RemoveAll(x => x.EdgeType != EdgeType.SingleUnit && groupedVertexes.Contains(x.SourceVertex));
                innerVertex.ForwardEdges.RemoveAll(x => x.EdgeType != EdgeType.SingleUnit && groupedVertexes.Contains(x.DestinationVertex));
                var outGroupBackEdges = innerVertex.BackEdges.Where(x => x.EdgeType != EdgeType.SingleUnit);
                foreach(var outBackEdge in outGroupBackEdges.ToArray())
                {
                    outBackEdge.DestinationVertex = this;
                    innerVertex.BackEdges.Remove(outBackEdge);
                    foreach (var backForwardEdge in outBackEdge.SourceVertex.ForwardEdges.Where(x => x.DestinationVertex == innerVertex))
                    {
                        backForwardEdge.DestinationVertex = this;
                    }
                    if (!BackEdges.Any(x => x.SourceVertex == outBackEdge.SourceVertex && x.EdgeType == outBackEdge.EdgeType && x.Index == outBackEdge.Index))
                    {
                        BackEdges.Add(outBackEdge);
                    }
                }
                var outGroupForwardEdges = innerVertex.ForwardEdges.Where(x => x.EdgeType != EdgeType.SingleUnit);
                foreach (var outForwardEdge in outGroupForwardEdges.ToArray())
                {
                    outForwardEdge.SourceVertex = this;
                    innerVertex.ForwardEdges.Remove(outForwardEdge);
                    foreach (var forwardBackEdge in outForwardEdge.SourceVertex.BackEdges.Where(x => x.SourceVertex == innerVertex))
                    {
                        forwardBackEdge.SourceVertex = this;
                    }
                    if (!ForwardEdges.Any(x => x.SourceVertex == outForwardEdge.SourceVertex && x.EdgeType == outForwardEdge.EdgeType && x.Index == outForwardEdge.Index))
                    {
                        ForwardEdges.Add(outForwardEdge);
                    }
                }
            }
        }
        public List<LabeledVertex> AdditionalVertexes = new List<LabeledVertex>();
    }
}
