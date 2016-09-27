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

namespace DoppleGraph
{
    public partial class Form1 : Form
    {
        private List<GoNodeWrapper> nodeWrappers;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AssemblyDefinition myLibrary = AssemblyDefinition.ReadAssembly(@"C:\Users\Simco\Documents\Visual Studio 2015\Projects\Dopple\Utility\bin\Release\Utility.dll");

            TypeDefinition type = myLibrary.MainModule.Types[1];

            foreach (var method in type.Methods.Where(x => !x.IsConstructor))
            {
                Form newForm = new Form();
                BackTraceManager backTraceManager = new BackTraceManager(method);
                var instructionWrappers = backTraceManager.Run();

                newForm.Text = method.Name;
                // create a Go view (a Control) and add to the form
                GoView myView = new GoView();
                myView.AllowDelete = false;
                myView.Dock = DockStyle.Fill;
                newForm.Controls.Add(myView);

                nodeWrappers =
                    instructionWrappers.Select(x => new GoNodeWrapper(new GoBasicNode(), x)).ToList();

                foreach (var goNodeWrapper in nodeWrappers)
                {
                    goNodeWrapper.Index = nodeWrappers.IndexOf(goNodeWrapper);
                    goNodeWrapper.Node.Shape.BrushColor = Color.Blue;
                    goNodeWrapper.Node.Shape = new GoRectangle();
                    goNodeWrapper.Node.Text = goNodeWrapper.InstructionWrapper.Instruction.OpCode.Code.ToString() + " "
                                              + nodeWrappers.IndexOf(goNodeWrapper) + 
                                              goNodeWrapper.InstructionWrapper.Instruction.Operand?.ToString() +
                                              " Stack: " + goNodeWrapper.InstructionWrapper.StackSum;
                    if (
                        new[] { Code.Call, Code.Calli, Code.Callvirt }.Contains(
                            goNodeWrapper.InstructionWrapper.Instruction.OpCode.Code))
                    {
                        goNodeWrapper.Node.Text += ((MethodReference)goNodeWrapper.InstructionWrapper.Instruction.Operand).Name ?? " ";
                    }
                    myView.Document.Add(goNodeWrapper.Node);
                }
                Random rnd = new Random();

                int RColorVal = 100;
                int GColorVal = 100;
                int BColorVal = 100;
                foreach (var nodeWrapper in nodeWrappers)
                { 
                    foreach (InstructionWrapper wrapper in nodeWrapper.InstructionWrapper.BackDataFlowRelated)
                    {
                        GoLink link = new GoLink();
                        link.Relinkable = false;
                        link.FromPort = nodeWrapper.Node.Port;
                        link.PenWidth = 3;
                        link.FromArrow = true;
                        var backNode = nodeWrappers.First(x => x.InstructionWrapper == wrapper).Node;
                        link.ToPort = backNode.Port;
                        myView.Document.Add(link);
                        GetColor(ref RColorVal, ref GColorVal, ref BColorVal);
                        link.PenColor = Color.FromArgb(RColorVal, GColorVal, BColorVal);
                    }

                    continue;
                    foreach (InstructionWrapper wrapper in nodeWrapper.InstructionWrapper.BackProgramFlow)
                    {
                        Color randomColor;
                        GoLink link = new GoLink();
                        link.FromArrow = true;
                        link.FromPort = nodeWrapper.Node.Port;
                        link.Pen = new Pen(link.Pen.Brush) {DashStyle = DashStyle.Dash};
                        link.PenWidth = 1;
                        link.Style = GoStrokeStyle.RoundedLineWithJumpGaps;
                        link.BrushStyle = GoBrushStyle.EllipseGradient;
                        var backNode = nodeWrappers.First(x => x.InstructionWrapper == wrapper).Node;
                        link.ToPort = backNode.Port;
                        myView.Document.Add(link);
                        randomColor = Color.Black;
                        link.PenColor = randomColor;
                    }
                    
                }
                AddColNumbers(nodeWrappers);
                newForm.Show();
            }
        }

        private void AddColNumbers(List<GoNodeWrapper> nodeWrappers)
        {
            Dictionary<int, List<GoNodeWrapper>> nodeWrapperCols = new Dictionary<int, List<GoNodeWrapper>>();
            var colNodes = nodeWrappers.Where(x => x.InstructionWrapper.BackDataFlowRelated.Count == 0).ToList();
            SetFirstColRows(colNodes);
            int iterations = 0;
            while (colNodes.Count > 0 && iterations < 20)
            {
                iterations++;
                colNodes = colNodes
                    .SelectMany(x => x.InstructionWrapper.ForwardDataFlowRelated)
                    .Select(x => nodeWrappers.First(y => y.InstructionWrapper == x))
                    .ToList();
                foreach (var colNode in colNodes)
                {
                    colNode.DisplayCol = colNode.InstructionWrapper.BackDataFlowRelated.Select(x => GetNodeWrapper(x).DisplayCol).Max() +1;
                    colNode.DisplayRow = colNode.InstructionWrapper.BackDataFlowRelated.Select(x => GetNodeWrapper(x)).Average(x => x.DisplayRow);
                }
            }

            int totalHeight = 1000;
            int totalWidth = 1500;
            float heightOffset = totalHeight / nodeWrappers.Select(x => x.DisplayRow).Max();
            float widthOffset = totalWidth / nodeWrappers.Select(x => x.DisplayCol).Max();
            foreach (var nodeWrapper in nodeWrappers)
            {
                nodeWrapper.Node.Location = new PointF(nodeWrapper.DisplayCol * widthOffset, nodeWrapper.DisplayRow * heightOffset);
            }
        }

        private GoNodeWrapper GetNodeWrapper (InstructionWrapper instWrapper)
        {
            return nodeWrappers.First(x => x.InstructionWrapper == instWrapper);
        }
        private void SetFirstColRows(List<GoNodeWrapper> colNodes)
        {
            int i = 0;
            var visited = new List<GoNodeWrapper>();
            foreach (var node in colNodes)
            {
                node.DisplayCol = 0;
                if (visited.Contains(node))
                    continue;
                var neighbours = colNodes
                    .Where(x => x.InstructionWrapper.ForwardDataFlowRelated.Intersect(node.InstructionWrapper.ForwardDataFlowRelated)
                    .Count() > 0);
                foreach (var neighbour in neighbours)
                {
                    neighbour.DisplayRow = i;
                    i++;
                }
                visited.AddRange(neighbours);
            }
        }

        private void GetColor(ref int RValue, ref int GValue, ref int BValue)
        {
            int increment = 20;
            int maxValue = 256;

            RValue += increment;
            if (RValue > maxValue)
            {
                RValue -= maxValue;
                GValue += increment;
                if (GValue > maxValue)
                {
                    GValue -= maxValue;
                    BValue += increment;
                }
            }
        }
    }
}
