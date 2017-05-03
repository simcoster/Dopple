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
                    else if (i!=j)
                    //else if (true)
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
            //if (Graph1 != Graph2)
            if (true)
            {
                var newFormm = new NodePairingGraph(pairing2);
                newFormm.Show();
                var newFormmm = new NodePairingGraph(pairing1);
                newFormmm.Show();
            }
            double Score1 = pairing1.TotalScore;
            double Score2 = pairing2.TotalScore;
            Console.WriteLine("{0} = {1} {2}", Graph1[0].Method.Name, Graph2[0].Method.Name, (Score1+ Score2)/ (pairing1.SourceSelfScore.TotalScore + pairing1.ImageSelfScore.TotalScore));
            return 0;
        }
    }
}
