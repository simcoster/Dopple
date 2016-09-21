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

            TypeDefinition type = myLibrary.MainModule.Types[1];

            BackTraceManager backTraceManager = new BackTraceManager(type.Methods[0]);
            var instructionWrappers = backTraceManager.Run();
          

            this.Text = "Minimal GoDiagram App";
            // create a Go view (a Control) and add to the form
            GoView myView = new GoView();
            myView.Dock = DockStyle.Fill;
            this.Controls.Add(myView);

            List<GoNodeWrapper> nodeWrappers =
                instructionWrappers.Select(x => new GoNodeWrapper(new GoBasicNode(), x)).ToList();

            int offset = 0;
            foreach (var goNodeWrapper in nodeWrappers)
            {
                goNodeWrapper.Index = nodeWrappers.IndexOf(goNodeWrapper);
                goNodeWrapper.Node.Shape.BrushColor = Color.Blue;
                goNodeWrapper.Node.Shape= new GoRectangle();
                goNodeWrapper.Node.Text = goNodeWrapper.InstructionWrapper.Instruction.OpCode.Code.ToString() + " " + nodeWrappers.IndexOf(goNodeWrapper);
                myView.Document.Add(goNodeWrapper.Node);
            }
            Random rnd = new Random();
            foreach (var nodeWrapper in nodeWrappers)
            {
                Color randomColor = Color.Blue;
                bool firstCon = true;

                AddLineNumber(nodeWrapper, nodeWrappers);
                nodeWrapper.Node.Location = new PointF(100 + offset, nodeWrapper.LineNum.Value *40);
                offset += 70;

                foreach (InstructionWrapper wrapper in nodeWrapper.InstructionWrapper.BackDataFlowRelated)
                {
                    GoLink link = new GoLink();
                    link.FromPort = nodeWrapper.Node.Port;
                    link.PenWidth = 3;
                    var backNode = nodeWrappers.First(x => x.InstructionWrapper == wrapper).Node;
                    link.ToPort = backNode.Port;
                    myView.Document.Add(link);
                    if (!firstCon)
                    {
                        randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                    }
                    link.PenColor = randomColor;
                    firstCon = false;
                }
            }
        }

        private void AddLineNumber(GoNodeWrapper nodeWrapper, List<GoNodeWrapper> nodeWrappers)
        {
            if (nodeWrappers.Count(x => x.LineNum != null) == 0 )
            {
                nodeWrapper.LineNum = 0;
                return;
            }
            var newLineNum = nodeWrappers.Where(x => x.LineNum != null).Select(x => x.LineNum.Value).Max() + 1;

            switch (nodeWrapper.InstructionWrapper.BackDataFlowRelated.Count)
            {
                case 0:
                    nodeWrapper.LineNum = newLineNum;
                    return;
                default:
                    nodeWrapper.LineNum =
                        nodeWrappers.Where(x => nodeWrapper.InstructionWrapper.BackDataFlowRelated.Contains(x.InstructionWrapper) && x.LineNum != null)
                        .Select(x => x.LineNum)
                        .Average();
                    return;
            }
        }
    }
}
