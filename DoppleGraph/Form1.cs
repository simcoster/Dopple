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
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AssemblyDefinition myLibrary = AssemblyDefinition.ReadAssembly(@"C:\Users\Simco\Documents\Visual Studio 2015\Projects\Dopple\Utility\bin\Release\Utility.dll");

            TypeDefinition type = myLibrary.MainModule.Types[1];

            foreach (var method in type.Methods.Where(x => !x.IsConstructor && x.Name.ToLower().Contains("for")))
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

                List<GoNodeWrapper> nodeWrappers =
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
                foreach (var nodeWrapper in nodeWrappers)
                {
                    nodeWrapper.Node.Location = new PointF(nodeWrapper.ColNum* 100 , nodeWrapper.LineNum * 100);
                }
                newForm.Show();
            }
            
        }

        private void AddColNumbers(List<GoNodeWrapper> nodeWrappers)
        {
            var firstNodes = nodeWrappers.Where(x => x.InstructionWrapper.BackDataFlowRelated.Count == 0).ToArray();
            int lineNum = 0;
            foreach (var firstOrderNode in firstNodes)
            {
                firstOrderNode.LineNum = lineNum;
                firstOrderNode.ColNum = 0;
                lineNum++;
                SetColNumRec(firstOrderNode, nodeWrappers);
            }
        }

        private void SetColNumRec(GoNodeWrapper nodeWrapper, List<GoNodeWrapper> nodeWrappers)
        {
            float offset = 0;
            foreach (InstructionWrapper instructionWrapper in nodeWrapper.InstructionWrapper.ForwardDataFlowRelated)
            {
                var forwardNode = nodeWrappers.First(x => x.InstructionWrapper == instructionWrapper);
                offset += 0.5f;
                forwardNode.LineNum = nodeWrapper.LineNum + offset;
                if (forwardNode.Index < nodeWrapper.Index)
                {
                    continue;
                }
                if (forwardNode.ColNum <= nodeWrapper.ColNum)
                {
                    forwardNode.ColNum = nodeWrapper.ColNum + 1;
                }
                else
                {
                    forwardNode.ColNum++;
                }
                SetColNumRec(forwardNode, nodeWrappers);
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
