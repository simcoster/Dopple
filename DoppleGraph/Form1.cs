using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DoppleTry2;
using Mono.Cecil;
using DoppleTry2.InstructionNodes;
using System.Diagnostics;
using GraphSimilarityByMatching;

namespace DoppleGraph
{
    public partial class Form1 : Form
    {
     
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AssemblyDefinition myLibrary = AssemblyDefinition.ReadAssembly(@"C:\Windows\assembly\GAC_32\mscorlib\2.0.0.0__b77a5c561934e089\mscorlib.dll");


            TypeDefinition type = myLibrary.MainModule.Types.First(x => x.FullName == "System.Array");

            var Graphs = new List<List<InstructionNode>>();
            //foreach (var method in type.Methods.Where(x => !x.IsConstructor))
            foreach (var method in type.Methods.Where(x => x.Name.Contains("Reverse")).Take(1))
            {
                var backTraceManager = new GraphBuilder(method);
                List<InstructionNode> instructionWrappers = backTraceManager.Run();
                Graphs.Add(instructionWrappers);
                var newForm = new Form2(instructionWrappers);
                newForm.Show();
            }

            AssemblyDefinition mysecondLibrary = AssemblyDefinition.ReadAssembly(@"C:\Users\Simco\Documents\Visual Studio 2015\Projects\Dopple\Utility\bin\release\Utility.dll");


            TypeDefinition typee = mysecondLibrary.MainModule.Types.First(x => x.Name == "Class1");

            var Graphss = new List<List<InstructionNode>>();
            //foreach (var method in type.Methods.Where(x => !x.IsConstructor))
            foreach (var method in typee.Methods.Where(x => !x.IsConstructor))
            {
                var backTraceManager = new GraphBuilder(method);
                List<InstructionNode> instructionWrappers = backTraceManager.Run();
                Graphs.Add(instructionWrappers);
                var newForm = new Form2(instructionWrappers);
                newForm.Show();
            }
            NewMethod(Graphs.GetRange(0, 2));
            //NewMethod(Graphs.GetRange(1, 2));


            //for (int i = 0; i < 50; i++)
            //{
            //    GraphSimilarityCalc.GetDistance(Graphs[0], Graphs[0], out pairingsOut);
            //    var score = PairingValidator.ScorePairings(pairingsOut);
            //    if (score > bestScore)
            //    {
            //        bestScore = score;
            //        bestMatch = pairingsOut;
            //    }
            //}
            //var newFormm = new NodePairingGraph(bestMatch, bestScore);
            //newFormm.Show();
        }

        private static void NewMethod(List<List<InstructionNode>> Graphs)
        {
            var biggerGraph = Graphs.OrderByDescending(x => x.Count).First();
            NodePairings bestMatch = null;
            var bestScore = 0;
            for (int i = 0; i < 50; i++)
            {
                NodePairings pairing = GraphSimilarityCalc.GetDistance(Graphs[0], Graphs[0]);
                if (pairing.Score > bestScore)
                {
                    bestScore = pairing.Score;
                    bestMatch = pairing;
                }
            }
            var fullSelfScore =  GraphSimilarityCalc.GetSelfScore(Graphs[0]);
            var newFormmm = new NodePairingGraph(bestMatch, bestScore);
            newFormmm.Show();
        }
    }
}
