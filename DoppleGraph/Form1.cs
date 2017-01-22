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
            AssemblyDefinition myLibrary = AssemblyDefinition.ReadAssembly(@"C:\Users\Simco\Documents\Visual Studio 2015\Projects\Dopple\Utility\bin\Release\Utility.dll");

            TypeDefinition type = myLibrary.MainModule.Types[3];

            var Graphs = new List<List<InstructionNode>>();
            foreach (var method in type.Methods.Where(x => !x.IsConstructor))
            {
                var backTraceManager = new GraphBuilder(method);
                List<InstructionNode> instructionWrappers = backTraceManager.Run();
                Graphs.Add(instructionWrappers);
                var newForm = new Form2(instructionWrappers);
                newForm.Show();
            }
            var biggerGraph = Graphs.GetRange(0,2).OrderByDescending(x => x.Count).First();
            Dictionary<LabeledVertex, List<LabeledVertex>> bestMatch = null;
            Dictionary<LabeledVertex, List<LabeledVertex>> pairingsOut;
            double bestScore = 0;
            for (int i=0; i<50; i++)
            {
                GraphSimilarityCalc.GetDistance(Graphs[0], Graphs[1], out pairingsOut);
                var score = PairingValidator.ScorePairings(pairingsOut);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMatch = pairingsOut;
                }
            }
            for (int i = 0; i < 50; i++)
            {
                GraphSimilarityCalc.GetDistance(Graphs[0], Graphs[0], out pairingsOut);
                var score = PairingValidator.ScorePairings(pairingsOut);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMatch = pairingsOut;
                }
            }
            Console.WriteLine(bestScore);
        }
    }
}
