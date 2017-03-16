using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dopple;
using Mono.Cecil;
using System.Linq;
using System.Collections.Generic;
using Dopple.InstructionNodes;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace GraphBuilderTests
{
    [TestClass]
    public class UnitTest1
    {
        Dictionary<string, List<InstructionNode>> GeneratedNodes = new Dictionary<string, List<InstructionNode>>();
        Dictionary<string, List<InstructionNode>> PrecomputedNodes = new Dictionary<string, List<InstructionNode>>();
        [TestInitialize]
        public void LoadFiles()
        {
            AssemblyDefinition utilityLibrary = AssemblyDefinition.ReadAssembly(@"C:\Users\Simco\Documents\Visual Studio 2015\Projects\Dopple\TestedFunctions\bin\Release\TestedFunctions.dll");
            TypeDefinition testClass = utilityLibrary.MainModule.Types.First(x => x.Name == "Class1");
            foreach(var method in testClass.Methods)
            {
                var nodes = new GraphBuilder(method).FullRunForTesting();
                GeneratedNodes.Add(method.Name, nodes);
            }

            DataContractSerializer ser =
              new DataContractSerializer(typeof(List<InstructionNode>));
            foreach (var file in Directory.EnumerateFiles("C:\\temp\\tests\\"))
            {
                using (var reader =new FileStream(file, FileMode.Open))
                {
                    List<InstructionNode> nodes = (List<InstructionNode>) ser.ReadObject(reader);
                    PrecomputedNodes.Add(Path.GetFileNameWithoutExtension(file), nodes);
                }
            }
        }
        [TestMethod]
        public void TestSimpleAdd()
        {
            
        }
    }
}
