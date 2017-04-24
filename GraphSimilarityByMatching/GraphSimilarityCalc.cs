using Dopple;
using Dopple.InstructionNodes;
using Mono.Cecil.Cil;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarityByMatching
{
    public class GraphSimilarityCalc
    {
        private static readonly CodeInfo opCodeInfo = new CodeInfo();


        public static NodePairings GetDistance(List<InstructionNode> sourceGraph, List<InstructionNode> imageGraph)
        {
            List<LabeledVertex> sourceGraphLabeled = GetLabeled(sourceGraph);
            List<LabeledVertex> imageGraphLabeled = GetLabeled(imageGraph);
            NodePairings bestMatch = GetPairings(sourceGraphLabeled, imageGraphLabeled);
            object lockObject = new object();
            //TODO change back to 10
            Parallel.For(1, 2, (i) =>
            {
                NodePairings pairing = GetPairings(sourceGraphLabeled, imageGraphLabeled);
                lock(lockObject)
                {
                    if (pairing.TotalScore > bestMatch.TotalScore)
                    {
                        bestMatch = pairing;
                    }
                }
            });
            return bestMatch ;
        }

        private static NodePairings GetPairings(List<LabeledVertex> sourceGraphLabeled, List<LabeledVertex> imageGraphLabeled)
        {
            NodePairings nodePairings = new NodePairings(sourceGraphLabeled, imageGraphLabeled);

            Random rnd = new Random();
            Dictionary<Code[], IEnumerable<LabeledVertex>> sourceGraphGrouped;
            Dictionary<Code[], IEnumerable<LabeledVertex>> imageGraphGrouped;
            GetGrouped(sourceGraphLabeled, imageGraphLabeled, out sourceGraphGrouped, out imageGraphGrouped);


            //Parallel.ForEach(sourceGraphGrouped.Keys, (codeGroup) =>
            foreach (var codeGroup in sourceGraphGrouped.Keys)
            {
                //not gonna lock for now
                foreach (var sourceGraphVertex in sourceGraphGrouped[codeGroup])
                {
                    Tuple<LabeledVertex, int> winningPair;
                    if (!imageGraphGrouped.ContainsKey(codeGroup))
                    {
                        winningPair = null;   
                    }
                    //Parallel.ForEach(imageGraphGrouped[codeGroup].OrderBy(x => rnd.Next()).ToList(), (imageGraphCandidate) =>
                    
                    else
                    {
                        var vertexPossiblePairings = new ConcurrentBag<Tuple<LabeledVertex, int>>();
                        foreach (var imageGraphCandidate in imageGraphGrouped[codeGroup].OrderBy(x => rnd.Next()).ToList())
                        {
                            vertexPossiblePairings.Add(new Tuple<LabeledVertex, int>(imageGraphCandidate, VertexScorer.GetScore(sourceGraphVertex, imageGraphCandidate, nodePairings)));
                        }
                        winningPair = vertexPossiblePairings.Where(x => x.Item2 > 0).OrderByDescending(x => x.Item2).ThenBy(x => rnd.Next()).FirstOrDefault();
                    }
                    //});
                    if (winningPair != null)
                    {
                        int winningPairScore = winningPair.Item2;
                        lock (nodePairings.Pairings)
                        {
                            nodePairings.Pairings[winningPair.Item1].Add(new SingleNodePairing(sourceGraphVertex, winningPairScore));
                            nodePairings.TotalScore += winningPairScore;
                        }
                    }
                    else
                    {
                        var selfPairing = VertexScorer.GetSelfScore(sourceGraphVertex);
                        nodePairings.TotalScore -= selfPairing;
                    }
                }
            }   
           // });
                
            return nodePairings;
        }

        private static void GetGrouped(List<LabeledVertex> sourceGraphLabeled, List<LabeledVertex> imageGraphLabeled, out Dictionary<Code[],IEnumerable<LabeledVertex>> sourceGraphGrouped, out Dictionary<Code[], IEnumerable<LabeledVertex>> imageGraphGrouped)
        {
            sourceGraphGrouped = CodeGroups.CodeGroupLists.AsParallel().ToDictionary(x => x, x => sourceGraphLabeled.Where(y => x.Contains(y.Opcode)));
            imageGraphGrouped = CodeGroups.CodeGroupLists.AsParallel().ToDictionary(x => x, x => imageGraphLabeled.Where(y => x.Contains(y.Opcode)));
            foreach (var singleCode in sourceGraphLabeled.Except(sourceGraphGrouped.Values.SelectMany(x => x)).GroupBy(x => x.Opcode))
            {
                var singleCodeArray = new Code[] { singleCode.Key };
                sourceGraphGrouped.Add(singleCodeArray, singleCode.ToList());
                imageGraphGrouped.Add(singleCodeArray, imageGraphLabeled.Where(x => x.Opcode == singleCode.Key).ToList());
            }
            return;
        }

        private static List<LabeledVertex> GetLabeled(List<InstructionNode> graph)
        {
            var labeledVertexes = graph.AsParallel().Select(x => new LabeledVertex()
            {
                Opcode = x.Instruction.OpCode.Code,
                Operand = x.Instruction.Operand,
                Index = x.InstructionIndex,
                Method = x.Method,
            }).ToList();
            Parallel.ForEach(graph, (node) =>
            {
                AddEdges(node, labeledVertexes);
            });
            return labeledVertexes;
        }

        private static List<LabeledVertex> GetBackSingleUnitTree(LabeledVertex frontMostInSingleUnit)
        {
            List<LabeledVertex> backTree = new List<LabeledVertex>();
            Queue<LabeledVertex> backRelated = new Queue<LabeledVertex>();
            foreach (var backSingleUnit in frontMostInSingleUnit.BackEdges.Where(x => x.EdgeType == EdgeType.SingleUnit))
            {
                backRelated.Enqueue(backSingleUnit.SourceVertex);
            }
            while (backRelated.Count > 0)
            {
                var currNode = backRelated.Dequeue();
                backTree.Add(currNode);
                foreach(var backSingleUnit in currNode.BackEdges.Where(x => x.EdgeType == EdgeType.SingleUnit))
                {
                    backRelated.Enqueue(backSingleUnit.SourceVertex);
                }
            }
            return backTree;
        }

        private static void AddEdges(InstructionNode node, List<LabeledVertex> labeledVertexes)
        {

            LabeledVertex vertex = labeledVertexes[node.InstructionIndex];
            foreach (var dataFlowBackVertex in node.DataFlowBackRelated)
            {
                vertex.BackEdges.Add(new LabeledEdge()
                {
                    EdgeType = EdgeType.DataFlow,
                    Index = dataFlowBackVertex.ArgIndex,
                    SourceVertex = labeledVertexes[dataFlowBackVertex.Argument.InstructionIndex],
                    DestinationVertex = vertex
                });
            }
            foreach (var branch in node.BranchProperties.Branches)
            {
                vertex.BackEdges.Add(new LabeledEdge()
                {
                    EdgeType = EdgeType.ProgramFlowAffecting,
                    Index = (int) branch.PairedBranchesIndex,
                    SourceVertex = labeledVertexes[branch.OriginatingNode.InstructionIndex],
                    DestinationVertex = vertex
                });
            }
            foreach (var dataFlowBackVertex in node.DataFlowForwardRelated)
            {
                vertex.ForwardEdges.Add(new LabeledEdge()
                {
                    EdgeType = EdgeType.DataFlow,
                    Index = dataFlowBackVertex.ArgIndex,
                    SourceVertex = vertex,
                    DestinationVertex = labeledVertexes[dataFlowBackVertex.Argument.InstructionIndex]
                });
            }
        }

        public static NodePairings GetSelfScore(List<LabeledVertex> labeledGraph)
        {
            NodePairings nodePairings = new NodePairings(labeledGraph, labeledGraph);
            foreach (var node in labeledGraph)
            {
                int score = VertexScorer.GetSelfScore(node);              
                nodePairings.Pairings[node].Add(new SingleNodePairing(node, score));
                nodePairings.TotalScore += score;
            }
            return nodePairings;
        }

      
    }

    internal enum SharedSourceOrDest
    {
        Source,
        Dest
    }
}
