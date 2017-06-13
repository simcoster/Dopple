using Dopple;
using Microsoft.CSharp;
using Mono.Cecil;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Dopple.InstructionNodes;
using Mono.Cecil.Cil;
using System.Drawing;

namespace DoppleWebDemo.Controllers.Helpers
{
    public static class GraphCreator
    {
        private const string CodePadding = @"using System;
                                            using System.Collections.Generic;
                                            using System.Linq;

                                            namespace TestedFunctions
                                            {
                                                class TestedFunction
                                                {
                                                    FunctionPlaceholder
                                                }
                                            }
                                            ";
        private static CodeColorHanlder CodeColorHandler = new CodeColorHanlder();
        public static NodesAndEdges CompileCode(string source)
        {
            Dictionary<string, string> providerOptions = new Dictionary<string, string>()
                {
                    {"CompilerVersion", "v4.0"}
                };
            CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);

            //var tempFile = Path.GetTempPath() + Guid.NewGuid().ToString() + ".dll";
            var tempFile = Path.GetTempPath() + "Functions" + ".dll";
            CompilerParameters compilerParams = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll", "System.dll" }, tempFile)
            {
                GenerateInMemory = false,
                GenerateExecutable = false
            };
            source = source.Trim();
            string firstLine = source.Substring(0, source.IndexOf('\n'));
            if (firstLine.Contains('(') && firstLine.Contains(')'))
            {
                source = CodePadding.Replace("FunctionPlaceholder", source);
            }
            CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, source);

            //if (results.Errors.Count != 0)
            //    throw new Exception("Mission failed!");

            AssemblyDefinition loadedAssembly = AssemblyDefinition.ReadAssembly(tempFile);
            if (loadedAssembly.MainModule.Types.Count(x => x.HasMethods) != 1)
            {
                throw new Exception("only one class allowed");
            }
            TypeDefinition loadedType = loadedAssembly.MainModule.Types.First(x => x.HasMethods);
            var graphBuilder = new GraphBuilder(loadedType.Methods.First());
            graphBuilder.Run();

            var nodesAndEdgesForJS =  GetNodesAndEdges(graphBuilder.InstructionNodes);
            nodesAndEdgesForJS.Method = graphBuilder.InstructionNodes[0].Method;
            nodesAndEdgesForJS.Nodes = graphBuilder.InstructionNodes;
            File.Delete(tempFile);
            return nodesAndEdgesForJS;
        }

        private static NodesAndEdges GetNodesAndEdges(List<InstructionNode> instructionNodes)
        {
            List<NodeForJS> nodes = GetNodes(instructionNodes);
            List<EdgeForJS> edges = GetEdges(instructionNodes);
            return new NodesAndEdges() { NodesJS = nodes, DataEdgesJS = edges };
        }

        private static List<NodeForJS> GetNodes(List<InstructionNode> instructionNodes)
        {
            return instructionNodes.Select(x => new NodeForJS() {
                key = x.InstructionIndex,
                color = ColorTranslator.ToHtml(CodeColorHandler.GetNodeCodeColor(x.Instruction.OpCode.Code)),
                text = GetInstructionText(x),
                method = x.Method.FullName}).ToList();
        }

        private static List<EdgeForJS> GetEdges(List<InstructionNode> instructionNodes)
        {
            var dataEdges = instructionNodes.SelectMany(x => x.DataFlowBackRelated.Select(y => new EdgeForJS()
            { from = y.Argument.InstructionIndex,
                to = x.InstructionIndex,
                color = ColorTranslator.ToHtml(CodeColorHandler.GetEdgeColor(y.ArgIndex, GraphSimilarityByMatching.EdgeType.DataFlow)),
                type = GraphSimilarityByMatching.EdgeType.DataFlow,
                isLayoutPositioned = true
            }));
            //var flowEdges = instructionNodes.SelectMany(x => x.BranchProperties.Branches.Select(y => new EdgeForJS()
            //{
            //    from = y.OriginatingNode.InstructionIndex,
            //    to = x.InstructionIndex,
            //    color = ColorTranslator.ToHtml(CodeColorHandler.GetEdgeColor(y.OriginatingNodeIndex, GraphSimilarityByMatching.EdgeType.ProgramFlowAffecting)),
            //    type = GraphSimilarityByMatching.EdgeType.ProgramFlowAffecting,
            //    isLayoutPositioned = false
            //}));
            var flowEdges = new List<EdgeForJS>();
            return dataEdges.Concat(flowEdges).ToList();

        }

        private static string GetInstructionText(InstructionNode x)
        {
            return x.Instruction.OpCode.Code.ToString() + x.Instruction.Operand ?? "";
        }
    }
}