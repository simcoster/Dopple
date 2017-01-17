using DoppleTry2.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarityByMatching
{
    public class Class1
    {
        public int GetDistance(List<InstructionNode> firstGraph, List<InstructionNode> secondGraph)
        {
            var sourceGraph = firstGraph.Count >= secondGraph.Count ? firstGraph : secondGraph;
            var destGraph = sourceGraph == firstGraph ? secondGraph : firstGraph;

            List<LabeledVertex> sourceGraphLabeled = GetLabeled(sourceGraph);
        }

        private List<LabeledVertex> GetLabeled(List<InstructionNode> graph)
        {
            var labeledVertexes = new List<LabeledVertex>();
            foreach (var instructionNode in graph)
            {
                LabeledVertex vertex = new LabeledVertex();
                vertex.Opcode = instructionNode.Instruction.OpCode.Code;
                vertex.Operand = instructionNode.Instruction.Operand;
                foreach (var dataFlowEdge in instructionNode.DataFlowBackRelated)
                {
                    vertex.BackEdges.Add(new LabledEdge()
                    {
                        EdgeType = EdgeType.DataFlow,
                        Index = dataFlowEdge.ArgIndex,
                        SourceVertexOpcode = dataFlowEdge.Argument.Instruction.OpCode.Code,
                        DestVertexOpcode = vertex.Opcode
                    });
                }
                foreach (var programFlowBackAffected in instructionNode.ProgramFlowBackAffected)
                {
                    vertex.BackEdges.Add(new LabledEdge()
                    {
                        EdgeType = EdgeType.ProgramFlow,
                        Index = programFlowBackAffected.ArgIndex,
                        SourceVertexOpcode = programFlowBackAffected.Argument.Instruction.OpCode.Code,
                        DestVertexOpcode = vertex.Opcode
                    });
                }
                foreach (var dataFlowEdge in instructionNode.DataFlowForwardRelated)
                {
                    vertex.BackEdges.Add(new LabledEdge()
                    {
                        EdgeType = EdgeType.DataFlow,
                        Index = dataFlowEdge.ArgIndex,
                        SourceVertexOpcode = dataFlowEdge.Argument.Instruction.OpCode.Code,
                        DestVertexOpcode = vertex.Opcode
                    });
                }
            }
        }
    }
}
