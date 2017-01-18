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
        private static readonly CodeInfo opCodeInfo = new CodeInfo();
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
                foreach(var candidate in candidates)
                {
                    GetScore(candidate, sourceVertex, pairings);
                }
            }
            return 0;
        }

        private static int GetScore(LabeledVertex firstVertex, LabeledVertex secondVertex, Dictionary<LabeledVertex, List<LabeledVertex>> pairings)
        {
            int score = 1;
            if (firstVertex.Opcode == secondVertex.Opcode)
            {
                score += 2;
            }
            if (opCodeInfo.GetIndexImportance()
        }

        private static Dictionary<LabeledEdge,LabeledEdge> PairEdges(List<LabeledEdge> firstEdges, List<LabeledEdge> secondEdges, Dictionary<LabeledVertex, List<LabeledVertex>> pairings)
        {
            var edgePairings = new Dictionary<LabeledEdge, LabeledEdge>();
            var unmachedSecondEdges = new List<LabeledEdge>(secondEdges);
            foreach(var firstEdge in firstEdges)
            {
                var pairingScores = new Dictionary<LabeledEdge, int>();
                Func<LabeledEdge,bool> predicate = x => x.EdgeType == firstEdge.EdgeType;
                if (opCodeInfo.GetIndexImportance(firstEdge.SourceVertexOpcode) == IndexImportance.Critical)
                {
                    predicate = x => x.EdgeType == firstEdge.EdgeType && x.Index == firstEdge.Index;
                }
                else
                {
                    predicate = x => x.EdgeType == firstEdge.EdgeType;
                }
                var relevantSecond = secondEdges.Where(predicate);
                if (relevantSecond)
            }
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
                    vertex.BackEdges.Add(new LabeledEdge()
                    {
                        EdgeType = EdgeType.DataFlow,
                        Index = dataFlowBackVertex.ArgIndex,
                        SourceVertexOpcode = dataFlowBackVertex.Argument.Instruction.OpCode.Code,
                        DestVertexOpcode = vertex.Opcode
                    });
                }
                foreach (var programFlowBackVertex in instructionNode.ProgramFlowBackAffected)
                {
                    vertex.BackEdges.Add(new LabeledEdge()
                    {
                        EdgeType = EdgeType.ProgramFlow,
                        Index = programFlowBackVertex.ArgIndex,
                        SourceVertexOpcode = programFlowBackVertex.Argument.Instruction.OpCode.Code,
                        DestVertexOpcode = vertex.Opcode
                    });
                }
                foreach (var dataFlowBackVertex in instructionNode.DataFlowForwardRelated)
                {
                    vertex.ForwardEdges.Add(new LabeledEdge()
                    {
                        EdgeType = EdgeType.DataFlow,
                        Index = dataFlowBackVertex.ArgIndex,
                        SourceVertexOpcode = dataFlowBackVertex.Argument.Instruction.OpCode.Code,
                        DestVertexOpcode = vertex.Opcode
                    });
                }
                foreach (var programFlowForwardVertex in instructionNode.ProgramFlowForwardAffecting)
                {
                    vertex.ForwardEdges.Add(new LabeledEdge()
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
