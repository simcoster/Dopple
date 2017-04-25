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
            var retNodes = sourceGraph.Where(x => x.Instruction.OpCode.Code == Code.Ret);
            var backRetTree = retNodes.SelectMany(x => BackSearcher.GetBackDataTree(x)).Distinct().Concat(retNodes).Select(x => x.InstructionIndex).ToList();
            foreach(var backRetNode in sourceGraphLabeled.Where(x => backRetTree.Contains(x.Index)))
            {
                backRetNode.IsInReturnBackTree = true;
            }
            NodePairings bestMatch = GetPairings(sourceGraphLabeled, imageGraphLabeled);
            object lockObject = new object();
            //TODO change back to 10
            Parallel.For(1, 1, (i) =>
            {
                NodePairings pairing = GetPairings(sourceGraphLabeled, imageGraphLabeled);
                if (pairing.TotalScore >1)
                { throw new Exception(""); }
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
            NodePairings nodePairings = new NodePairings(sourceGraph: sourceGraphLabeled, imageGraph: imageGraphLabeled);

            Random rnd = new Random();
            Dictionary<Code[], IEnumerable<LabeledVertex>> sourceGraphGrouped;
            Dictionary<Code[], IEnumerable<LabeledVertex>> imageGraphGrouped;
            GetGrouped(sourceGraphLabeled: sourceGraphLabeled, imageGraphLabeled: imageGraphLabeled, sourceGraphGrouped: out sourceGraphGrouped, imageGraphGrouped: out imageGraphGrouped);


            //Parallel.ForEach(sourceGraphGrouped.Keys, (codeGroup) =>
            foreach (var codeGroup in sourceGraphGrouped.Keys)
            {
                //not gonna lock for now
                foreach (var sourceGraphVertex in sourceGraphGrouped[codeGroup])
                {
                    VertexMatch winningPair;
                    if (!imageGraphGrouped.ContainsKey(codeGroup))
                    {
                        winningPair = null;   
                    }
                    //Parallel.ForEach(imageGraphGrouped[codeGroup].OrderBy(x => rnd.Next()).ToList(), (imageGraphCandidate) =>
                    
                    else
                    {
                        var vertexPossiblePairings = new ConcurrentBag<VertexMatch>();
                        foreach (var imageGraphCandidate in imageGraphGrouped[codeGroup].OrderBy(x => rnd.Next()).ToList())
                        {
                            vertexPossiblePairings.Add(new VertexMatch() { SourceGraphVertex = sourceGraphVertex, ImageGraphVertex = imageGraphCandidate, Score = VertexScorer.GetScore(sourceGraphVertex, imageGraphCandidate, nodePairings) });
                        }
                        winningPair = vertexPossiblePairings.Where(x => x.Score > 0).OrderByDescending(x => x.Score).ThenBy(x => rnd.Next()).FirstOrDefault();
                    }
                    //});
                    if (winningPair != null)
                    {
                        lock (nodePairings.Pairings)
                        {
                            winningPair.NormalizedScore = winningPair.Score / VertexScorer.GetSelfScore(winningPair.SourceGraphVertex); 
                            nodePairings.Pairings[winningPair.ImageGraphVertex].Add(winningPair);
                            nodePairings.TotalScore += winningPair.Score;
                        }
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

        public static NodePairings GetGraphSelfScore(List<LabeledVertex> labeledGraph)
        {
            NodePairings nodePairings = new NodePairings(imageGraph: labeledGraph, sourceGraph: labeledGraph);
            foreach (var node in labeledGraph)
            {
                double score = VertexScorer.GetSelfScore(node);              
                nodePairings.Pairings[node].Add(new VertexMatch() { SourceGraphVertex = node, ImageGraphVertex = node, Score = score, NormalizedScore = 1 });
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
