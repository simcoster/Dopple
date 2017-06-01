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
                var graphBuilder = new GraphBuilder(method);
                List<InstructionNode> instructionNodes = graphBuilder.Run();
                Graphs.Add(instructionNodes);
                var newForm = new Form2(instructionNodes);
                newForm.Show();
            }

            WriteTalbe(Graphs, "oneway.csv", false);
            WriteTalbe(Graphs, "twoway.csv", true);
        }

        private static void WriteTalbe(List<List<InstructionNode>> Graphs, string fileName, bool twoWayMark)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            foreach (var graph in Graphs)
            {
                dt.Columns.Add(graph.First().Method.Name);
            }
            for (int i = 0; i < Graphs.Count; i++)
            {
                var row = dt.NewRow();
                row["name"] = Graphs[i].First().Method.Name;
                for (int j = 0; j < Graphs.Count; j++)
                {
                    row[Graphs[j].First().Method.Name] = Math.Round(GetContainedScore(Graphs[i], Graphs[j], twoWayMark), 4);
                }
                dt.Rows.Add(row);
            }
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText("c:\\temp\\" + fileName, sb.ToString());
        }

        private static double GetContainedScore(List<InstructionNode> Graph1, List<InstructionNode> Graph2, bool twoWayScore)
        {
            NodePairings pairing1 = GraphSimilarityCalc.GetDistance(Graph1, Graph2);

            double Graph1ContainedIn2Score = pairing1.TotalScore / pairing1.SourceSelfScore.TotalScore;

            if (twoWayScore)
            {
                NodePairings pairing2 = GraphSimilarityCalc.GetDistance(Graph2, Graph1);
                double totalScoreAverage = (pairing1.TotalScore + pairing2.TotalScore) / (pairing1.SourceSelfScore.TotalScore + pairing1.ImageSelfScore.TotalScore);
                return totalScoreAverage;
            }
            else
            {
                return Graph1ContainedIn2Score;
            }
        }
    }
}
