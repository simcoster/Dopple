using DoppleTry2;
using DoppleTry2.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarityByMatching
{
    public class GraphSimilarityCalc
    {
        public static int GetDistance(List<InstructionNode> firstGraph, List<InstructionNode> secondGraph)
        {
            var pairings = new Dictionary<LabeledVertex, List<LabeledVertex>>();
            var biggerGraph = firstGraph.Count >= secondGraph.Count ? firstGraph : secondGraph;
            var smallerGraph = biggerGraph == firstGraph ? secondGraph : firstGraph;

            List<LabeledVertex> biggerGraphLabeled = GetLabeled(biggerGraph);
            List<LabeledVertex> smallerGraphLabeled = GetLabeled(smallerGraph);
            
            foreach(var sourceVertex in biggerGraphLabeled)
            {
                var candidates = biggerGraphLabeled.Where(x => CodeGroups.AreSameGroup(x.Opcode, sourceVertex.Opcode));

            }
            return 0;
        }

        private static List<LabeledVertex> GetLabeled(List<InstructionNode> graph)
        {
            var labeledVertexes = new List<LabeledVertex>();
            foreach (var instructionNode in graph)
            {
                LabeledVertex vertex = new LabeledVertex();
                vertex.Opcode = instructionNode.Instruction.OpCode.Code;
                vertex.Operand = instructionNode.Instruction.Operand;
                foreach (var dataFlowBackVertex in instructionNode.DataFlowBackRelated)
                {
                    vertex.BackEdges.Add(new LabledEdge()
                    {
                        EdgeType = EdgeType.DataFlow,
                        Index = dataFlowBackVertex.ArgIndex,
                        SourceVertexOpcode = dataFlowBackVertex.Argument.Instruction.OpCode.Code,
                        DestVertexOpcode = vertex.Opcode
                    });
                }
                foreach (var programFlowBackVertex in instructionNode.ProgramFlowBackAffected)
                {
                    vertex.BackEdges.Add(new LabledEdge()
                    {
                        EdgeType = EdgeType.ProgramFlow,
                        Index = programFlowBackVertex.ArgIndex,
                        SourceVertexOpcode = programFlowBackVertex.Argument.Instruction.OpCode.Code,
                        DestVertexOpcode = vertex.Opcode
                    });
                }
                foreach (var dataFlowBackVertex in instructionNode.DataFlowForwardRelated)
                {
                    vertex.ForwardEdges.Add(new LabledEdge()
                    {
                        EdgeType = EdgeType.DataFlow,
                        Index = dataFlowBackVertex.ArgIndex,
                        SourceVertexOpcode = dataFlowBackVertex.Argument.Instruction.OpCode.Code,
                        DestVertexOpcode = vertex.Opcode
                    });
                }
                foreach (var programFlowForwardVertex in instructionNode.ProgramFlowForwardAffecting)
                {
                    vertex.ForwardEdges.Add(new LabledEdge()
                    {
                        EdgeType = EdgeType.ProgramFlow,
                        Index = programFlowForwardVertex.ArgIndex,
                        SourceVertexOpcode = vertex.Opcode,
                        DestVertexOpcode = programFlowForwardVertex.Argument.Instruction.OpCode.Code
                    });
                }
                labeledVertexes.Add(vertex);
            }
            return labeledVertexes;
        }
    }
}
