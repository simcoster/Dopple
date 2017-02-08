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
            AssemblyDefinition myrLibrary = AssemblyDefinition.ReadAssembly(@"C:\Windows\assembly\GAC_MSIL\System.Core\3.5.0.0__b77a5c561934e089\system.core.dll");
            TypeDefinition type = myrLibrary.MainModule.Types.First(x => x.FullName == "System.Linq.Enumerable");

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
            for (int i = 0; i < Graphs.Count - 1; i++)
            {
                csv.Append(Graphs[i][0].Method.Name);
            }
            csv.AppendLine();
            for (int i =0; i<Graphs.Count-1; i++)
            {
                for (int j=0; j < Graphs.Count; j++)
                {
                    if (j==0)
                    {
                        csv.Append(Graphs[i][0].Method.Name); 
                    }
                    else if (j >= i + 1)
                    {
                        csv.Append(NewMethod(Graphs[i], Graphs[j]));
                    }
                    else if (j >= i + 1)
                    {
                        csv.Append(" ");
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
            double Score1 = (double) pairing1.Score / (double) GraphSimilarityCalc.GetSelfScore(pairing1.FirstGraph).Score;
            NodePairings pairing2 = GraphSimilarityCalc.GetDistance(Graph2, Graph1);
            double Score2 = (double) pairing2.Score / (double) GraphSimilarityCalc.GetSelfScore(pairing2.FirstGraph).Score;
            Console.WriteLine("{0} = {1} {2}", Graph1[0].Method.Name, Graph2[0].Method.Name, (Score1+Score2)/2 );
            return (Score1 + Score2) / 2;
            //var newFormmm = new NodePairingGraph(pairing, GraphSimilarityCalc.GetSelfScore(pairing.FirstGraph));
            //newFormmm.Show();
        }
    }
}
