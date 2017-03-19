using Dopple;
using Dopple.InstructionNodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GraphBuilderTests
{
    public class HelperFuncs
    {
        public static void TestFunction(string funcName)
        {
            AssemblyDefinition utilityLibrary = AssemblyDefinition.ReadAssembly(@"C:\Users\Simco\Documents\Visual Studio 2015\Projects\Dopple\TestedFunctions\bin\Release\TestedFunctions.dll");
            TypeDefinition testClass = utilityLibrary.MainModule.Types.First(x => x.Name == "Class1");
            List<InstructionNode> generatedNodes = null;
            generatedNodes = new GraphBuilder(testClass.Methods.First(x => x.Name == funcName)).FullRunForTesting();
            List<InstructionNode> precomputedNodes = null;
            DataContractSerializer ser =
              new DataContractSerializer(typeof(List<InstructionNode>));
            using (var reader = new FileStream("C:\\temp\\tests\\" + funcName + ".xml", FileMode.Open))
            {
                precomputedNodes = (List<InstructionNode>) ser.ReadObject(reader);
            }
            foreach (var node in precomputedNodes)
            {
                var equivilentNode = generatedNodes[node.InstructionIndex];
                foreach (var backDataArg in node.DataFlowBackRelated)
                {
                    Assert.IsTrue(equivilentNode.DataFlowBackRelated.Any(x => ArgumentsMatch(backDataArg, x)));
                }
                foreach (var forwardAffectingArg in node.ProgramFlowForwardAffecting)
                {
                    Assert.IsTrue(equivilentNode.ProgramFlowForwardAffecting.Any(x => ArgumentsMatch(forwardAffectingArg, x)));
                }
            }
        }

        private static bool ArgumentsMatch(IndexedArgument firstArg, IndexedArgument secondArg)
        {
            return firstArg.ArgIndex == secondArg.ArgIndex && firstArg.Argument.InstructionIndex == secondArg.Argument.InstructionIndex;
        }
    }
}
