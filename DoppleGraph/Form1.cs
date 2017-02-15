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
            AssemblyDefinition mysecondLibrary = AssemblyDefinition.ReadAssembly(@"C:\Users\Simco\Documents\Visual Studio 2015\Projects\Dopple\Utility\bin\Release\Utility.dll");
            TypeDefinition typee = mysecondLibrary.MainModule.Types.First(x => x.Name == "Class1");

            var Graphs = new List<List<InstructionNode>>();
            //foreach (var method in type.Methods.Where(x => !x.IsConstructor))
            foreach (var method in typee.Methods.Where(x => !x.IsConstructor))
            {
                var graphBuilder = new GraphBuilder(method);
                List<InstructionNode> instructionNodes = graphBuilder.Run();
                Graphs.Add(instructionNodes);
                var newForm = new Form2(instructionNodes);
                newForm.Show();
            }

            //AssemblyDefinition myrLibrary = AssemblyDefinition.ReadAssembly(@"C:\Windows\assembly\GAC_MSIL\System.Core\3.5.0.0__b77a5c561934e089\system.core.dll");
            //TypeDefinition type = myrLibrary.MainModule.Types.First(x => x.FullName == "System.Linq.Enumerable");

            ////foreach (var method in type.Methods.Where(x => !x.IsConstructor))
            //foreach (var method in type.Methods.Where(x => x.Name.Contains("Sum")).Take(1))
            //{
            //    var backTraceManager = new GraphBuilder(method);
            //    List<InstructionNode> instructionWrappers = backTraceManager.Run();
            //    //Graphs.Add(instructionWrappers);
            //    var newForm = new Form2(instructionWrappers);
            //    newForm.Show();
            //}'


            var csv = new StringBuilder();
            csv.Append(',');
            for (int i = 0; i < Graphs.Count ; i++)
            {
                csv.Append(Graphs[i][0].Method.Name);
                csv.Append(',');
            }
            csv.AppendLine();
            for (int i =0; i<Graphs.Count; i++)
            {
                for (int j=-1; j < Graphs.Count; j++)
                {
                    if (j==-1)
                    {
                        csv.Append(Graphs[i][0].Method.Name);
                    }
                    else
                    {
                        csv.Append(NewMethod(Graphs[i], Graphs[j]));
                    }
                    csv.Append(",");
                }
                csv.AppendLine();
            }
            File.WriteAllText("C:\\temp\\comparisons.csv", csv.ToString());
        }

        private static double NewMethod(List<InstructionNode> Graph1, List<InstructionNode> Graph2)
        {
            NodePairings pairing1 = GraphSimilarityCalc.GetDistance(Graph1, Graph2);
            NodePairings pairing2 = GraphSimilarityCalc.GetDistance(Graph2, Graph1);
            if (Graph1 == Graph2)
            {
                var newFormm = new NodePairingGraph(pairing2, GraphSimilarityCalc.GetSelfScore(pairing2.FirstGraph));
                newFormm.Show();
                var newFormmm = new NodePairingGraph(pairing1, GraphSimilarityCalc.GetSelfScore(pairing1.FirstGraph));
                newFormmm.Show();
            }
            double Score1 = pairing1.Score;
            double Score2 = pairing2.Score;
            Console.WriteLine("{0} = {1} {2}", Graph1[0].Method.Name, Graph2[0].Method.Name, (Score1 + Score2) / 2);
            double selfScoreSum = GraphSimilarityCalc.GetSelfScore(pairing1.FirstGraph).Score + GraphSimilarityCalc.GetSelfScore(pairing1.SecondGraph).Score;
            return Math.Round((Score1 +Score2) / selfScoreSum, 2);
        }
    }
}
