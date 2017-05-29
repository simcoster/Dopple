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
        private static CodeColorHanlder CodeColorHandler = new CodeColorHanlder();
        public static NodesAndEdges CompileCode(string source)
        {
            source = @"using System.Linq;
            class Program {
              public static void Main(string[] args) {
                var q = from i in Enumerable.Range(1,100)
                          where i % 2 == 0
                          select i;
              }
            }";
            Dictionary<string, string> providerOptions = new Dictionary<string, string>()
                {
                    {"CompilerVersion", "v4.0"}
                };
            CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);

            var tempFile = Path.GetTempPath() + Guid.NewGuid().ToString() + ".dll";
            CompilerParameters compilerParams = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll", "System.dll" }, tempFile)
            {
                GenerateInMemory = false,
                GenerateExecutable = false
            };

            CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, source);

            if (results.Errors.Count != 0)
                throw new Exception("Mission failed!");

            AssemblyDefinition loadedAssembly = AssemblyDefinition.ReadAssembly(tempFile);
            if (loadedAssembly.MainModule.Types.Count(x => x.HasMethods) != 1)
            {
                throw new Exception("only one class allowed");
            }
            TypeDefinition loadedType = loadedAssembly.MainModule.Types.First(x => x.HasMethods);
            if (loadedType.Methods.Count != 3)
            {
                throw new Exception("only one user method allowed");
            }

            var graphBuilder = new GraphBuilder(loadedType.Methods.First());
            graphBuilder.Run();

            return GetNodesAndEdges(graphBuilder.InstructionNodes);
        }

        private static NodesAndEdges GetNodesAndEdges(List<InstructionNode> instructionNodes)
        {
            List<NodeForJS> nodes = instructionNodes.Select(x => new NodeForJS() { key = x.InstructionIndex, color = ColorTranslator.ToHtml(CodeColorHandler.GetNodeCodeColor(x.Instruction.OpCode.Code)),text = x.Instruction.OpCode.Code.ToString() }).ToList();
            List<EdgeForJS> edges = instructionNodes.SelectMany(x => x.DataFlowBackRelated.Select(y => new EdgeForJS() { from = y.Argument.InstructionIndex, to = x.InstructionIndex, color = ColorTranslator.ToHtml(CodeColorHandler.GetEdgeColor(y.ArgIndex, GraphSimilarityByMatching.EdgeType.DataFlow)) })).ToList();
            return new NodesAndEdges() { Nodes = nodes, Edges = edges };
        }

        private static string GetColorForCode(Code code)
        {
            throw new NotImplementedException();
        }
    }
}