using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Dopple;
using Mono.Cecil;
using Dopple.InstructionNodes;
using System.Diagnostics;
using GraphSimilarityByMatching;
using System.Text;
using System.IO;

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
            AssemblyDefinition mysecondLibrary = AssemblyDefinition.ReadAssembly(@"C:\Users\Simco\Documents\Visual Studio 2015\Projects\Dopple\TestedFunctions\bin\Release\TestedFunctions.dll");
            TypeDefinition typee = mysecondLibrary.MainModule.Types.First(x => x.Name == "InProgress");

            var Graphs = new List<List<InstructionNode>>();
            //foreach (var method in type.Methods.Where(x => !x.IsConstructor))
            var defaultCtor = typee.Methods.First(x => x.Name == ".ctor");
            var typeInit = typee.Methods.FirstOrDefault(x => x.Name == ".cctor");
            foreach (var method in typee.Methods.Where(x => !x.IsConstructor && !x.IsAbstract))
            {
                var graphBuilder = new GraphBuilder(new FunctionFlowGraph(method, typeInit));
                List<InstructionNode> instructionNodes = graphBuilder.Run();
                Graphs.Add(instructionNodes);
                var newForm = new Form2(instructionNodes);
                newForm.Show();
            }

            var csv = new StringBuilder();
            for (int i =0; i<Graphs.Count; i++)
            {
                for (int j = i+1; j < Graphs.Count; j++)
                {
                    csv.Append(GetContainedScore(Graphs[i], Graphs[j]));
                }
                csv.AppendLine();
            }
            File.WriteAllText("C:\\temp\\comparisons.txt", csv.ToString());
        }

        private static string GetContainedScore(List<InstructionNode> Graph1, List<InstructionNode> Graph2)
        {
            NodePairings pairing1 = GraphSimilarityCalc.GetDistance(Graph1, Graph2);
            NodePairings pairing2 = GraphSimilarityCalc.GetDistance(Graph2, Graph1);
          
            double Graph1ContainedIn2Score = pairing1.TotalScore / pairing1.SourceSelfScore.TotalScore;
            double Graph2ContainedIn1Score = pairing2.TotalScore / pairing2.SourceSelfScore.TotalScore;
            double totalScoreAverage = (Graph1ContainedIn2Score + Graph2ContainedIn1Score) / (pairing1.SourceSelfScore.TotalScore + pairing1.ImageSelfScore.TotalScore);
            var newFormm = new NodePairingGraph(pairing2);
            newFormm.Show();
            var newFormmm = new NodePairingGraph(pairing1);
            newFormmm.Show();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0} --> {1} = {2}", pairing1.SourceGraph.First().Method.Name, pairing1.ImageGraph.First().Method.Name, Graph1ContainedIn2Score));
            sb.AppendLine(string.Format("{0} --> {1} = {2}", pairing2.SourceGraph.First().Method.Name, pairing2.ImageGraph.First().Method.Name, Graph2ContainedIn1Score));
            sb.AppendLine(string.Format("{0} <-> {1} = {2}", pairing1.SourceGraph.First().Method.Name, pairing1.ImageGraph.First().Method.Name, totalScoreAverage));
            return sb.ToString();
        }
    }
}
