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
        private const int UnmachedVertexPenalty = 5;

      
        public static NodePairing GetDistance(List<InstructionNode> firstGraph, List<InstructionNode> secondGraph)
        {
            NodePairing nodePairing = new NodePairing();
            var pairings = nodePairing.Pairings;

            List<InstructionNode> biggerGraph;
            List<InstructionNode> smallerGraph;
            if (firstGraph.Count > secondGraph.Count)
            {
                biggerGraph = firstGraph;
                smallerGraph = secondGraph;
            }
            else
            {
                biggerGraph = secondGraph;
                smallerGraph = firstGraph;
            }
            List<LabeledVertex> biggerGraphLabeled = GetLabeled(biggerGraph);
            List<LabeledVertex> smallerGraphLabeled = GetLabeled(smallerGraph);
            nodePairing.BigGraph = biggerGraphLabeled;
            nodePairing.SmallGraph = smallerGraphLabeled;

            smallerGraphLabeled.ForEach(x => pairings.Add(x, new List<LabeledVertex>()));

            int graphPairingScore = 0;
            Random rnd = new Random();
            foreach (var bigGraphVertex in biggerGraphLabeled.OrderBy(x => rnd.Next()))
            {
                var vertexPossiblePairings = new Dictionary<LabeledVertex, int>();
                var smallGraphCandidates = smallerGraphLabeled.Where(x => CodeGroups.AreSameGroup(x.Opcode, bigGraphVertex.Opcode)).ToList();
                foreach(var smallGraphCandidate in smallGraphCandidates.OrderBy(x => rnd.Next()))
                {
                    vertexPossiblePairings.Add(smallGraphCandidate, GetScore(smallGraphCandidate, bigGraphVertex, pairings, false));
                }
                var winningPairs = vertexPossiblePairings.GroupBy(x => x.Value).OrderByDescending(x => x.Key).FirstOrDefault();
                if (winningPairs == null)
                {
                    graphPairingScore -= UnmachedVertexPenalty;
                }
                else
                {
                    KeyValuePair<LabeledVertex, int> winningPair = winningPairs.ElementAt(rnd.Next(0, winningPairs.Count()));
                    graphPairingScore += winningPair.Value;
                    pairings[winningPair.Key].Add(bigGraphVertex);
                }
            }
            return nodePairing;
        }

        private static int GetScore(LabeledVertex smallGraphVertex, LabeledVertex bigGraphVertex, Dictionary<LabeledVertex, List<LabeledVertex>> pairings, bool userPairings)
        {
            int score = 1;
            if (smallGraphVertex.Opcode == bigGraphVertex.Opcode)
            {
                score += 1;
            }
            var backEdgeScore = EdgeScorer.ScoreEdges(smallGraphVertex.BackEdges, bigGraphVertex.BackEdges, pairings, SharedSourceOrDest.Dest);
            var forwardEdgeScore = EdgeScorer.ScoreEdges(smallGraphVertex.ForwardEdges, bigGraphVertex.ForwardEdges, pairings, SharedSourceOrDest.Source);
            score += backEdgeScore + forwardEdgeScore;
            score -= pairings[smallGraphVertex].Count;
            return score;
        }

      

        private static List<LabeledVertex> GetLabeled(List<InstructionNode> graph)
        {
            var labeledVertexes = new List<LabeledVertex>();
            foreach (var instructionNode in graph)
            {
                LabeledVertex vertex = new LabeledVertex();
                vertex.Opcode = instructionNode.Instruction.OpCode.Code;
                vertex.Operand = instructionNode.Instruction.Operand;
                vertex.Index = instructionNode.InstructionIndex;
                vertex.Method = instructionNode.Method;
                labeledVertexes.Add(vertex);
            }
            foreach (var instructionNode in graph)
            {
                LabeledVertex vertex = labeledVertexes[instructionNode.InstructionIndex];
                foreach (var dataFlowBackVertex in instructionNode.DataFlowBackRelated)
                {
                    vertex.BackEdges.Add(new LabeledEdge()
                    {
                        EdgeType = EdgeType.DataFlow,
                        Index = dataFlowBackVertex.ArgIndex,
                        SourceVertex = labeledVertexes[dataFlowBackVertex.Argument.InstructionIndex],
                        DestinationVertex = vertex
                    });
                }
                foreach (var programFlowBackVertex in instructionNode.ProgramFlowBackAffected)
                {
                    vertex.BackEdges.Add(new LabeledEdge()
                    {
                        EdgeType = EdgeType.ProgramFlowAffecting,
                        Index = programFlowBackVertex.ArgIndex,
                        SourceVertex = labeledVertexes[programFlowBackVertex.Argument.InstructionIndex],
                        DestinationVertex = vertex
                    });
                }
                foreach (var dataFlowBackVertex in instructionNode.DataFlowForwardRelated)
                {
                    vertex.ForwardEdges.Add(new LabeledEdge()
                    {
                        EdgeType = EdgeType.DataFlow,
                        Index = dataFlowBackVertex.ArgIndex,
                        SourceVertex = vertex,
                        DestinationVertex = labeledVertexes[dataFlowBackVertex.Argument.InstructionIndex]
                    });
                }
                foreach (var programFlowForwardVertex in instructionNode.ProgramFlowForwardAffecting)
                {
                    vertex.ForwardEdges.Add(new LabeledEdge()
                    {
                        EdgeType = EdgeType.ProgramFlowAffecting,
                        Index = programFlowForwardVertex.ArgIndex,
                        SourceVertex = vertex,
                        DestinationVertex = labeledVertexes[programFlowForwardVertex.Argument.InstructionIndex]
                    });
                }
            }
            return labeledVertexes;
        }
    }

    internal enum SharedSourceOrDest
    {
        Source,
        Dest
    }
}
