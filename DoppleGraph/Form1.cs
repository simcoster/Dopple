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
                myView.AllowInsert = false;
                myView.AllowLink = false;
                myView.Dock = DockStyle.Fill;
                newForm.Controls.Add(myView);

                nodeWrappers =
                    instructionWrappers
                    //.Where(x => x.ForwardDataFlowRelated.Count >0 || x.BackDataFlowRelated.Count >0)
                    .Select(x => new GoNodeWrapper(new GoTextNodeHoverable(), x))
                    .ToList();

                foreach (var goNodeWrapper in nodeWrappers)
                {
                    goNodeWrapper.Node.Selected += Node_Selected;
                    goNodeWrapper.Node.UnSelected += Node_UnSelected;
                    goNodeWrapper.Index = nodeWrappers.IndexOf(goNodeWrapper);
                    ((GoShape)goNodeWrapper.Node.Background).BrushColor = colorCode.GetColor(goNodeWrapper.InstructionWrapper.Instruction.OpCode.Code);
                    var shape = ((GoShape)goNodeWrapper.Node.Background);
                    shape.Size = new SizeF(400, 400);

                    //goNodeWrapper.Node.Shape.BrushColor = colorCode.GetColor(goNodeWrapper.InstructionWrapper.Instruction.OpCode.Code);
                    goNodeWrapper.Node.Text = goNodeWrapper.InstructionWrapper.Instruction.OpCode.Code.ToString() + " " +nodeWrappers.IndexOf(goNodeWrapper);

                    if ( new[] { Code.Call, Code.Calli, Code.Callvirt }.Contains(
                            goNodeWrapper.InstructionWrapper.Instruction.OpCode.Code))
                    {
                        goNodeWrapper.Node.Text += ((MethodReference)goNodeWrapper.InstructionWrapper.Instruction.Operand).Name ?? " ";
                    }
                    myView.Document.Add(goNodeWrapper.Node);
                }
                SetCoordinates(nodeWrappers);


                foreach (var nodeWrapper in nodeWrappers)
                {
                    AddNodeLinks(nodeWrapper, myView);
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
                        var backNode = GetNodeWrapper(wrapper);
                        link.ToPort = backNode.Node.RightPort;
                        myView.Document.Add(link);
                        randomColor = Color.Black;
                        link.PenColor = randomColor;
                    }
                    
                }
                newForm.Show();
            }
        }

        private List<GoNode> NodesToShow = new List<GoNode>();

        private void Node_UnSelected(object sender, EventArgs e)
        {
            lock (NodesToShow)
            {
                var node = (GoNode)sender;
                NodesToShow.Remove(node);
                ReShow(node.Document);
            }
          
        }

        private void ReShow(GoDocument doc)
        {
            if (NodesToShow.Intersect(doc).Count() ==0)
            {
                foreach (var goObject in doc)
                {
                    goObject.Visible = true;
                }
            }
            else
            {
                foreach (var goObject in doc)
                {
                    goObject.Visible = false;
                }
                foreach (GoNode node in NodesToShow)
                {
                    foreach(var link in node.Links)
                    {
                        ((GoLink)link).Visible = true;
                        ((GoNode)link.ToNode).Visible = true;
                        ((GoNode)link.FromNode).Visible = true;
                    }
                }
            }
        }

        private void Node_Selected(object sender, EventArgs e)
        {
            var node = (GoNode)sender;
            NodesToShow.Add(sender as GoNode);
            ReShow(node.Document);
        }

        public void AddNodeLinks(GoNodeWrapper nodeWrapper, GoView myView)
        {
            var linkGroupColor = Color.FromArgb(245, 228, 176);
            foreach (var argumentGroup in nodeWrapper.InstructionWrapper.BackDataFlowRelated.ArgumentList.GroupBy(x => x.ArgIndex))
            {
                linkGroupColor = GetNextColor(linkGroupColor);
                foreach (var indexedArg in argumentGroup)
                {
                    GoLink link = new GoLink();
                    var backNode = GetNodeWrapper(indexedArg.Argument);
                    link.PenColor = linkGroupColor;
                    link.ToPort = nodeWrapper.Node.LeftPort;
                    link.FromPort = backNode.Node.RightPort;
                    myView.Document.Add(link);
                    link.PenWidth = 3;

                }
            }
        }

        private Color GetNextColor(Color color)
        {
            return Color.FromArgb((color.R + 30) % 255, (color.G + 40) % 255, (color.B + 70) % 255);
        }

        private void SetCoordinates(List<GoNodeWrapper> nodeWrappers)
        {
            Dictionary<int, List<GoNodeWrapper>> nodeWrapperCols = new Dictionary<int, List<GoNodeWrapper>>();
            var firstNode = nodeWrappers.Where(x => x.InstructionWrapper.BackDataFlowRelated.ArgumentList.Count == 0).ToList();
            SetLongestPathRec(firstNode);
            SetRowIndexes(nodeWrappers);
            FixDuplicateCoordinates(nodeWrappers);
            int totalHeight = 1000;
            int totalWidth = 1000;
            float heightOffset = Convert.ToSingle(totalHeight / nodeWrappers.Select(x => x.DisplayRow).Max());
            float widthOffset = Convert.ToSingle(totalWidth / nodeWrappers.Select(x => x.DisplayCol).Max());
            foreach (var nodeWrapper in nodeWrappers)
            {
                nodeWrapper.Node.Location = new PointF(nodeWrapper.DisplayCol * widthOffset, (nodeWrapper.DisplayRow-0.7f) * heightOffset);
            }

        }

        private void FixDuplicateCoordinates(List<GoNodeWrapper> nodeWrappers)
        {
            for (int i = 0; i <= nodeWrappers.Select(x => x.DisplayCol).Max(); i++)
            {
                var colNodes = nodeWrappers.Where(x => x.DisplayCol == i).OrderBy(x => x.DisplayRow).ToList();
                colNodes[0].DisplayRow = (float)Math.Ceiling(colNodes[0].DisplayRow);
                int rowNum = (int)colNodes[0].DisplayRow +1;
                foreach (var node in colNodes.Skip(1))
                {
                    node.DisplayRow = rowNum;
                    rowNum++;
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
                var nodesToUpdate = node.InstructionWrapper.ForwardDataFlowRelated.ArgumentList
                    .Select(x => GetNodeWrapper(x.Argument))
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
            foreach (int col in allNodes.Select(x => x.DisplayCol).Distinct())
            {
                var orderedNodes = allNodes.Where(x => x.DisplayCol == col)
                    .OrderBy(x => x.InstructionWrapper.Instruction.OpCode.Code.ToString()).ToList();
                foreach (var node in orderedNodes)
                {
                    node.DisplayRow = orderedNodes.IndexOf(node);
                }
            }
        }
    }
}
