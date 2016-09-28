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

namespace DoppleGraph
{
    public partial class Form1 : Form
    {
        private List<GoNodeWrapper> nodeWrappers;
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Maybe swtich to graphVIZZZZ
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// </summary>
        private Dictionary<Code, Color> CodeColors = new Dictionary<Code, Color>();

        private void Form1_Load(object sender, EventArgs e)
        {
            AssemblyDefinition myLibrary = AssemblyDefinition.ReadAssembly(@"C:\Users\Simco\Documents\Visual Studio 2015\Projects\Dopple\Utility\bin\Release\Utility.dll");

            CodeColorHanlder colorCode = new CodeColorHanlder();
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
                    instructionWrappers
                    .Where(x => x.ForwardDataFlowRelated.Count >0 || x.BackDataFlowRelated.Count >0)
                    .Select(x => new GoNodeWrapper(new GoTextNode(), x))
                    .ToList();

                foreach (var goNodeWrapper in nodeWrappers)
                {
                    goNodeWrapper.Index = nodeWrappers.IndexOf(goNodeWrapper);
                    ((GoShape)goNodeWrapper.Node.Background).BrushColor = colorCode.GetColor(goNodeWrapper.InstructionWrapper.Instruction.OpCode.Code);
                    var shape = ((GoShape)goNodeWrapper.Node.Background);
                    shape.Size = new SizeF(400, 400);

                    //goNodeWrapper.Node.Shape.BrushColor = colorCode.GetColor(goNodeWrapper.InstructionWrapper.Instruction.OpCode.Code);
                    //goNodeWrapper.Node.Text = goNodeWrapper.InstructionWrapper.Instruction.OpCode.Code.ToString() + " "
                    //                          + goNodeWrapper.Index + " " +
                    //                          goNodeWrapper.InstructionWrapper.Instruction.Operand?.ToString();
                    if (
                        new[] { Code.Call, Code.Calli, Code.Callvirt }.Contains(
                            goNodeWrapper.InstructionWrapper.Instruction.OpCode.Code))
                    {
                        //goNodeWrapper.Node.Text += ((MethodReference)goNodeWrapper.InstructionWrapper.Instruction.Operand).Name ?? " ";
                    }
                    goNodeWrapper.Node.Text = "                 \n\n\n\n";
                    myView.Document.Add(goNodeWrapper.Node);
                }

                foreach (var nodeWrapper in nodeWrappers)
                { 
                    foreach (InstructionWrapper wrapper in nodeWrapper.InstructionWrapper.BackDataFlowRelated)
                    {
                        GoLink link = new GoLink();
                        link.Relinkable = false;
                        link.FromPort = nodeWrapper.Node.LeftPort;
                        link.PenWidth = 3;
                        link.FromArrow = true;
                        var backNode = nodeWrappers.First(x => x.InstructionWrapper == wrapper).Node;
                        link.ToPort = backNode.RightPort;
                        myView.Document.Add(link);
                        link.PenColor = Color.FromArgb(245,228,176);
                    }

                    continue;
                    foreach (InstructionWrapper wrapper in nodeWrapper.InstructionWrapper.BackProgramFlow)
                    {
                        Color randomColor;
                        GoLink link = new GoLink();
                        link.FromArrow = true;
                        link.FromPort = nodeWrapper.Node.LeftPort;
                        link.Pen = new Pen(link.Pen.Brush) {DashStyle = DashStyle.Dash};
                        link.PenWidth = 1;
                        link.Style = GoStrokeStyle.RoundedLineWithJumpGaps;
                        link.BrushStyle = GoBrushStyle.EllipseGradient;
                        var backNode = nodeWrappers.First(x => x.InstructionWrapper == wrapper).Node;
                        link.ToPort = backNode.RightPort;
                        myView.Document.Add(link);
                        randomColor = Color.Black;
                        link.PenColor = randomColor;
                    }
                    
                }
                SetCoordinates(nodeWrappers);
                newForm.Show();
            }
        }

        private void SetCoordinates(List<GoNodeWrapper> nodeWrappers)
        {
            Dictionary<int, List<GoNodeWrapper>> nodeWrapperCols = new Dictionary<int, List<GoNodeWrapper>>();
            var firstColNodes = nodeWrappers.Where(x => x.InstructionWrapper.BackDataFlowRelated.Count == 0).ToList();
            SetLongestPathRec(firstColNodes);
            SetRowIndexes(nodeWrappers);
            FixDuplicateCoordinates(nodeWrappers);
            int totalHeight = 1000;
            int totalWidth = 1000;
            float heightOffset = totalHeight / nodeWrappers.Select(x => x.DisplayRow).Max();
            float widthOffset = totalWidth / nodeWrappers.Select(x => x.DisplayCol).Max();
            foreach (var nodeWrapper in nodeWrappers)
            {
                nodeWrapper.Node.Location = new PointF(nodeWrapper.DisplayCol * widthOffset, (nodeWrapper.DisplayRow-0.7f) * heightOffset);
            }

        }

        private void FixDuplicateCoordinates(List<GoNodeWrapper> nodeWrappers)
        {
            var sameCoordiantesGroups = nodeWrappers.GroupBy(x => new { x.DisplayCol, x.DisplayRow }).Where(y => y.Count() >1);
            foreach (var group in sameCoordiantesGroups)
            {
                foreach (var node in group.Select(y => y).Skip(1))
                {
                    node.DisplayRow += 0.5f;
                }
            }
        }

        private GoNodeWrapper GetNodeWrapper (InstructionWrapper instWrapper)
        {
            return nodeWrappers.First(x => x.InstructionWrapper == instWrapper);
        }
        private void SetLongestPathRec(List<GoNodeWrapper> colNodes)
        {
            foreach (var node in colNodes)
            {
                var nodesToUpdate = node.InstructionWrapper.ForwardDataFlowRelated
                    .Select(x => GetNodeWrapper(x))
                    .Where(x => x.LongestPath.Count == 0 || !x.LongestPath.Intersect(node.LongestPath).SequenceEqual(x.LongestPath) )
                    .Where(x => x.LongestPath.Count < node.LongestPath.Count + 1)
                    .ToList();
                foreach (var nodeToUpdate in nodesToUpdate)
                {
                    nodeToUpdate.LongestPath = node.LongestPath.Concat(new[] { node }).ToList();
                    nodeToUpdate.DisplayCol = nodeToUpdate.LongestPath.Count;
                }
                SetLongestPathRec(nodesToUpdate);
            }
        }

        private void SetRowIndexes(List<GoNodeWrapper> allNodes)
        {
            var firstColNodes = allNodes.Where(x => x.InstructionWrapper.BackDataFlowRelated.Count == 0);
            var handledFirstNodes = new List<GoNodeWrapper>();
            handledFirstNodes.AddRange(firstColNodes);
            int rowNum = 1;
            while (handledFirstNodes.Count > 0)
            {
                var neighbours = handledFirstNodes
                                         .Where(x => x.InstructionWrapper.ForwardDataFlowRelated
                                         .Intersect(handledFirstNodes[0].InstructionWrapper.ForwardDataFlowRelated)
                                         .Count() > 0)
                                         .Concat(new[] { handledFirstNodes[0] })
                                         .Distinct()
                                         .ToList();
                foreach(var neighbour in neighbours)
                {
                    neighbour.DisplayRow = rowNum;
                    rowNum++;
                    handledFirstNodes.Remove(neighbour);
                }
            }
            foreach(var node in allNodes.Except(firstColNodes))
            {
                node.DisplayRow = node.InstructionWrapper.BackDataFlowRelated.Select(x => GetNodeWrapper(x)).Average(x => x.DisplayRow);
            }
        }

      
    }
}
