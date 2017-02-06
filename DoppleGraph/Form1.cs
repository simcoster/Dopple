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
            AssemblyDefinition mysecondLibrary = AssemblyDefinition.ReadAssembly(@"C:\Users\Simco\Documents\Visual Studio 2015\Projects\Dopple\Utility\bin\release\Utility.dll");
            TypeDefinition typee = mysecondLibrary.MainModule.Types.First(x => x.Name == "Class1");

            var Graphs = new List<List<InstructionNode>>();
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
        }

        private static void NewMethod(List<List<InstructionNode>> Graphs)
        {
            var biggerGraph = Graphs.OrderByDescending(x => x.Count).First();
            NodePairings bestMatch = null;
            var bestScore = 0;
            for (int i = 0; i < 5; i++)
            {
                NodePairings pairing = GraphSimilarityCalc.GetDistance(Graphs[0], Graphs[1]);
                if (pairing.Score > bestScore)
                {
                    bestScore = pairing.Score;
                    bestMatch = pairing;
                }
            }
            var fullSelfScore =  GraphSimilarityCalc.GetSelfScore(Graphs[0]);
            var newFormmm = new NodePairingGraph(bestMatch, ((double)bestScore/ (double) fullSelfScore)*100);
            newFormmm.Show();
        }
    }
}
