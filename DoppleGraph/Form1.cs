using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using DoppleTry2;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Northwoods.Go;
using DoppleTry2.BackTrackers;
using DoppleTry2.InstructionNodes;
using GraphSimilarity;
using System.Diagnostics;

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
            var problematics = Graphs.SelectMany(x => x).Where(x => x.DataFlowForwardRelated.Any(y => !y.DataFlowBackRelated.Any(z => z.Argument == x))).ToList();
            if (problematics.Count > 0)
            {
                Debugger.Break();
            }

            var editDistance = GraphEditDistanceCalc.GetEditDistance(Graphs[0], Graphs[1]);
        }
    }
}
