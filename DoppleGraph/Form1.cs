using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoppleTry2;
using DoppleTry2.BackTrackers;
using Mono.Cecil;
using Northwoods.Go;

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
            AssemblyDefinition myLibrary = AssemblyDefinition.ReadAssembly(@"C:\Users\Simco\documents\visual studio 2015\Projects\DoppleTry2\Utility\bin\Release\Utility.dll");

            TypeDefinition type = myLibrary.MainModule.Types[2];

            BackTraceManager backTraceManager = new BackTraceManager(type.Methods[0]);
            var instructionWrappers = backTraceManager.Run();
          

            this.Text = "Minimal GoDiagram App";
            // create a Go view (a Control) and add to the form
            GoView myView = new GoView();
            myView.Dock = DockStyle.Fill;
            this.Controls.Add(myView);

            List<GoNodeWrapper> nodeWrappers = new List<GoNodeWrapper>();

            foreach (var instructionWrapper in instructionWrappers)
            {
                GoBasicNode node1 = new GoBasicNode();
                nodeWrappers.Add(new GoNodeWrapper(node1, instructionWrapper));

                node1.
                // specify position, label and color
                node1.Location = new PointF(100, 100);
                node1.Text = "first";
                node1.Editable = true;  // first node is editable with F2 only
                node1.Shape.BrushColor = Color.Blue;
                // add to the document, not to the view
                myView.Document.Add(node1);
            }

            GoBasicNode node2 = new GoBasicNode();
            node2.Location = new PointF(200, 100);
            node2.Text = "second";
            node2.Label.Editable = true;  // second node is editable by clicking only
            node2.Shape.BrushColor = Color.Magenta;
            myView.Document.Add(node2);
        }
    }
}
