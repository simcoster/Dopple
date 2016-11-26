using DoppleTry2;
using DoppleTry2.InstructionWrappers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Northwoods.Go;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoppleGraph
{
    public partial class Form2 : Form
    {
        public List<GoNodeWrapper> nodeWrappers;
        public Dictionary<int, Color> ColumnBaseColors = new Dictionary<int, Color>();
        public GoView myView;
        CodeColorHanlder colorCode = new CodeColorHanlder();

        private Form2()
        {
            InitializeComponent();
        }

        public Form2(List<InstructionWrapper> instructionWrappers)
        {
            this.instructionWrappers = instructionWrappers;
            InitializeComponent();
        }

        private List<GoNode> NodesToShow = new List<GoNode>();
        private List<GoObject> ObjectsToHide = new List<GoObject>();
        private Random rnd = new Random();
        private List<InstructionWrapper> instructionWrappers;

        private void Node_UnSelected(object sender, EventArgs e)
        {
            lock (NodesToShow)
            {
                if (maxIndex.Text != "" || minIndex.Text != "")
                {
                    return;
                }
                if (myView.Selection.Count == 0)
                {
                    NodesToShow.Clear();
                }
                var node = (GoNode)sender;
                NodesToShow.Remove(node);
                ReShow();
            }

        }

        private void ReShow()
        {
            if (NodesToShow.Intersect(myView.Document).Count() == 0)
            {
                foreach (var goObject in myView.Document.Except(ObjectsToHide))
                {
                    goObject.Visible = true;
                }
                ShowDataFlowLinks_CheckedChanged(null, null);
                ShowFlowLinks_CheckedChanged(null, null);
            }
            else
            {
                foreach (var goObject in myView.Document)
                {
                    goObject.Visible = false;
                }
                foreach (GoNode node in NodesToShow)
                {
                    var linksToShow = node.Links.Cast<GoLink>().ToList();
                    if (!ShowProgramFlowLinks.Checked)
                    {
                        linksToShow = linksToShow.Cast<GoLink>().Where(x => x.Pen.DashStyle != DashStyle.Dash).ToList();
                    }
                    if (!ShowDataFlowLinks.Checked)
                    {
                        linksToShow = linksToShow.Cast<GoLink>().Where(x => x.Pen.DashStyle != DashStyle.Solid).ToList();
                    }
                    foreach (var link in linksToShow.Except(ObjectsToHide).Cast<GoLink>())
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
            if (ModifierKeys.HasFlag(Keys.Shift))
            {
                NodesToShow.AddRange(nodeWrappers.Where(x => x.InstructionWrapper.Method == GetNodeWrapper(node).InstructionWrapper.Method)
                    .Select(x => x.Node));
            }
            NodesToShow.Add(sender as GoNode);
            ReShow();
        }

        private void DrawFlowLinks(GoNodeWrapper nodeWrapper, GoView myView)
        {
            foreach (InstructionWrapper wrapper in nodeWrapper.InstructionWrapper.NextPossibleProgramFlow)
            {
                Color randomColor;
                GoLink link = new GoLink();
                link.FromArrow = true;
                link.ToPort = nodeWrapper.Node.RightPort;
                link.Pen = new Pen(link.Pen.Brush) { DashStyle = DashStyle.Dash };
                link.PenWidth = 1;
                link.Style = GoStrokeStyle.RoundedLineWithJumpGaps;
                link.BrushStyle = GoBrushStyle.EllipseGradient;
                var backNode = GetNodeWrapper(wrapper);
                link.FromPort = backNode.Node.LeftPort;
                myView.Document.Add(link);
                randomColor = Color.Black;
                link.PenColor = randomColor;
            }
        }

        private void AcomodateFlowForRemovedNodes()
        {
            foreach (var instWrapper in instructionWrappers)
            {
                instWrapper.NextPossibleProgramFlow = GetStillExistingNextFlowInstructions(instWrapper).ToList();
            }
        }

        public IEnumerable<InstructionWrapper> GetStillExistingNextFlowInstructions(InstructionWrapper instruction, List<InstructionWrapper> visited =null)
        {
            if (visited == null)
            {
                visited = new List<InstructionWrapper>();
            }
            visited.Add(instruction);
            List<InstructionWrapper> wrappersToReturn = new List<InstructionWrapper>();
            foreach(var nextInst in instruction.NextPossibleProgramFlow)
            {
                if (visited.Contains(nextInst))
                {
                    continue;
                }
                else if (instructionWrappers.Contains(nextInst))
                {
                    wrappersToReturn.Add(nextInst);
                }
                else
                {
                    wrappersToReturn.AddRange(GetStillExistingNextFlowInstructions(nextInst,visited));
                }
            }
            return wrappersToReturn;
        }

        public void DrawDataLinks(GoNodeWrapper nodeWrapper, GoView myView)
        {
            if (!ColumnBaseColors.ContainsKey(nodeWrapper.DisplayCol))
            {
                ColumnBaseColors.Add(nodeWrapper.DisplayCol, GetRandomColor());
            }
            //var linkGroupColor = ColumnBaseColors[nodeWrapper.DisplayCol];
            foreach (var argumentGroup in nodeWrapper.InstructionWrapper.BackDataFlowRelated.ArgumentList.GroupBy(x => x.ArgIndex))
            {
                var linkGroupColor = GetRandomColor();
                foreach (var indexedArg in argumentGroup)
                {
                    GoLink link = new GoLink();
                    var backNode = GetNodeWrapper(indexedArg.Argument);
                    link.BrushColor = linkGroupColor;
                    link.PenColor = linkGroupColor;
                    link.ToolTipText = indexedArg.ArgIndex.ToString() + " " + link.PenColor.R;
                    link.ToPort = nodeWrapper.Node.LeftPort;
                    link.FromPort = backNode.Node.RightPort;
                    myView.Document.Add(link);
                    link.PenWidth = 3;
                }
            }
            var allLinks = myView.Document.Where(x => x is GoLink).Cast<GoLink>().Select(y => y.PenColor);
        }

        private Color GetRandomColor()
        {
            Color color = new Color();
            color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            return color;
            //return Color.FromArgb((color.R + 30) % 255, (color.G + 20) % 255, (color.B + 10) % 255);
        }

        private void SetCoordinates(List<GoNodeWrapper> nodeWrappers)
        {
            Dictionary<int, List<GoNodeWrapper>> nodeWrapperCols = new Dictionary<int, List<GoNodeWrapper>>();
            var firstNode = nodeWrappers.Where(x => x.InstructionWrapper.BackDataFlowRelated.ArgumentList.Count == 0).ToList();
            SetLongestPathRec(firstNode);
            SetRowIndexes(nodeWrappers);
            FixDuplicateCoordinates(nodeWrappers);
            int totalHeight = 1000;
            int totalWidth = 2000;
            float heightOffset = Convert.ToSingle(totalHeight / nodeWrappers.Select(x => x.DisplayRow).Max());
            float widthOffset = Convert.ToSingle(totalWidth / nodeWrappers.Select(x => x.DisplayCol).Max());
            foreach (var nodeWrapper in nodeWrappers)
            {
                nodeWrapper.Node.Location = new PointF(nodeWrapper.DisplayCol * widthOffset, (nodeWrapper.DisplayRow - 0.7f) * heightOffset);
            }

        }

        private void FixDuplicateCoordinates(List<GoNodeWrapper> nodeWrappers)
        {
            for (int i = 0; i <= nodeWrappers.Select(x => x.DisplayCol).Max(); i++)
            {
                var colNodes = nodeWrappers.Where(x => x.DisplayCol == i).OrderBy(x => x.DisplayRow).ToList();
                colNodes[0].DisplayRow = (float)Math.Ceiling(colNodes[0].DisplayRow);
                int rowNum = (int)colNodes[0].DisplayRow + 1;
                foreach (var node in colNodes.Skip(1))
                {
                    node.DisplayRow = rowNum;
                    rowNum++;
                }
            }
        }

        private GoNodeWrapper GetNodeWrapper(InstructionWrapper instWrapper)
        {
            return nodeWrappers.First(x => x.InstructionWrapper == instWrapper);
        }

        private GoNodeWrapper GetNodeWrapper(GoNode goNode)
        {
            return nodeWrappers.First(x => x.Node == goNode);
        }

        private void SetLongestPathRec(List<GoNodeWrapper> colNodes)
        {
            foreach (var node in colNodes)
            {
                try
                {
                    var nodesToUpdate = node.InstructionWrapper.ForwardDataFlowRelated.ArgumentList
                   .Select(x => GetNodeWrapper(x.Argument))
                   .Where(x => x.LongestPath.Count == 0 || !x.LongestPath.Intersect(node.LongestPath).SequenceEqual(x.LongestPath))
                   .Where(x => x.LongestPath.Count < node.LongestPath.Count + 1)
                   .ToList();
                    foreach (var nodeToUpdate in nodesToUpdate)
                    {
                        nodeToUpdate.LongestPath = node.LongestPath.Concat(new[] { node }).ToList();
                        nodeToUpdate.DisplayCol = nodeToUpdate.LongestPath.Count;
                    }
                    SetLongestPathRec(nodesToUpdate);
                }
               catch
                {
                }
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

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Text = instructionWrappers[0].Method.Name;
            // create a Go view (a Control) and add to the form
            myView = new GoView();
            myView.KeyPress += MyView_KeyPress;
            myView.AllowDelete = false;
            myView.AllowInsert = false;
            myView.AllowLink = false;
            myView.Dock = DockStyle.Fill;
            this.Controls.Add(myView);
            myView.Show();

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
                var shape = ((GoShape)goNodeWrapper.Node.Background);
                shape.BrushColor = colorCode.GetColor(goNodeWrapper.InstructionWrapper.Instruction.OpCode.Code);
                if (shape.BrushColor.GetBrightness() < 0.4)
                {
                    goNodeWrapper.Node.Label.TextColor = Color.White;
                }
                shape.Size = new SizeF(400, 400);

                if (goNodeWrapper.InstructionWrapper.InliningProperties.Inlined || goNodeWrapper.InstructionWrapper.MarkForDebugging)
                {
                    if (goNodeWrapper.InstructionWrapper.MarkForDebugging)
                    {
                        goNodeWrapper.Node.Shape.PenColor = Color.Red;
                    }
                    else
                    {
                        goNodeWrapper.Node.Shape.PenColor = Color.Blue;
                    }
                    goNodeWrapper.Node.Shape.PenWidth = 3;
                    goNodeWrapper.Node.Shadowed = true;
                    goNodeWrapper.Node.ToolTipText = goNodeWrapper.InstructionWrapper.Method.Name + "*************";
                }

                goNodeWrapper.Node.Text = goNodeWrapper.InstructionWrapper.Instruction.OpCode.Code.ToString() + " index:" + goNodeWrapper.InstructionWrapper.InstructionIndex + " offset:" + goNodeWrapper.InstructionWrapper.Instruction.Offset;

                if (new[] { Code.Call, Code.Calli, Code.Callvirt }.Contains(
                        goNodeWrapper.InstructionWrapper.Instruction.OpCode.Code))
                {
                    goNodeWrapper.Node.Text += ((MethodReference)goNodeWrapper.InstructionWrapper.Instruction.Operand).Name ?? " ";
                }
                else if (goNodeWrapper.InstructionWrapper.Instruction.Operand != null)
                {
                    goNodeWrapper.Node.Text += goNodeWrapper.InstructionWrapper.Instruction.Operand.ToString();
                }
                else if ( goNodeWrapper.InstructionWrapper is FunctionArgInstWrapper)
                {
                    var ArgName = ((FunctionArgInstWrapper)goNodeWrapper.InstructionWrapper).ArgName;
                    goNodeWrapper.Node.Text += " " + ArgName + " ";
                }

                if (goNodeWrapper.InstructionWrapper.InliningProperties.Recursive)
                {
                    goNodeWrapper.Node.Text += " RecIndex:" + goNodeWrapper.InstructionWrapper.InliningProperties.RecursionInstanceIndex;
                }
                var frontLayer = myView.Document.Layers.CreateNewLayerAfter(myView.Document.LinksLayer);
                frontLayer.Add(goNodeWrapper.Node);
            }
            SetCoordinates(nodeWrappers);


            foreach (var nodeWrapper in nodeWrappers)
            {
                DrawDataLinks(nodeWrapper, myView);
                AcomodateFlowForRemovedNodes();
                DrawFlowLinks(nodeWrapper, myView);
            }
            //MarkLoops(nodeWrappers);

        }

        private void MarkLoops(List<GoNodeWrapper> nodeWrappers)
        {
            foreach(var node in nodeWrappers.Where(x => x.InstructionWrapper.BackProgramFlow.Count ==0))
            {

            }
        }

        private void MyView_KeyPress(object sender, KeyPressEventArgs e)
        {
            var backTraceManager = new GraphBuilder(instructionWrappers);
            if (e.KeyChar == '/')
            {
                PermanentlyHideSelection();
            }
            else if (e.KeyChar == '.')
            {
                foreach(var node in myView.Selection.Where(x => x is GoNode).Cast<GoNode>().ToArray())
                {
                    var nodeBackTree = BackSearcher.GetBackDataTree(GetNodeWrapper(node).InstructionWrapper).Select(x => GetNodeWrapper(x).Node).ToList();
                    var nodesToHide = myView.Document.Where(x => x is GoNode).Except(nodeBackTree).ToList();
                    foreach(var notABackNode in nodesToHide)
                    {
                        myView.Selection.Add(notABackNode);
                    }
                    myView.Selection.Remove(node);
                }
                PermanentlyHideSelection();
            }
            else if (e.KeyChar == '*')
            {
                ObjectsToHide.Clear();
                NodesToShow.Clear();
                ReShow();
            }
        }

        private void PermanentlyHideSelection()
        {
            var nodesToHide = myView.Selection
                                .Where(x => x is GoNode)
                                .Cast<GoNode>();
            foreach (var node in nodesToHide)
            {
                foreach (GoLink link in node.Links)
                {
                    link.Visible = false;
                    ObjectsToHide.Add(link);
                }
                node.Visible = false;
                ObjectsToHide.Add(node);
            }
        }

        private void ShowFlowLinks_CheckedChanged(object sender, EventArgs e)
        {
            var flowLinks = myView.Document.Where(x => x is GoLink).Cast<GoLink>().Where(x => x.Pen.DashStyle == DashStyle.Dash);
            foreach (var flowLink in flowLinks)
            {
                if (ShowProgramFlowLinks.Checked)
                {
                    flowLink.Visible = true;
                }
                else
                {
                    flowLink.Visible = false;
                }
            }
        }

        private void ShowDataFlowLinks_CheckedChanged(object sender, EventArgs e)
        {
            var flowLinks = myView.Document.Except(ObjectsToHide).Where(x => x is GoLink).Cast<GoLink>().Where(x => x.Pen.DashStyle != DashStyle.Dash);
            foreach (var flowLink in flowLinks)
            {
                if (ShowDataFlowLinks.Checked)
                {
                    flowLink.Visible = true;
                }
                else
                {
                    flowLink.Visible = false;
                }
            }
        }

        private void ShowRightDataLinks_CheckedChanged(object sender, EventArgs e)
        {
            var linksToHide = myView.Selection.Except(ObjectsToHide).Where(x => x is GoTextNode).Cast<GoTextNode>().SelectMany(x => x.LeftPort.Links).Cast<GoLink>();
            foreach (var flowLink in linksToHide)
            {
                if (ShowRightDataLinks.Checked)
                {
                    flowLink.Visible = true;
                }
                else
                {
                    flowLink.Visible = false;
                }
            }
        }

        private void ShowLeftDataLinks_CheckedChanged(object sender, EventArgs e)
        {
            var linksToHide = myView.Selection.Where(x => x is GoTextNode).Cast<GoTextNode>().SelectMany(x => x.RightPort.Links).Cast<GoLink>();
            foreach (var flowLink in linksToHide)
            {
                if (ShowLeftDataLinks.Checked)
                {
                    flowLink.Visible = true;
                }
                else
                {
                    flowLink.Visible = false;
                }
            }
        }

        private void minIndex_TextChanged(object sender, EventArgs e)
        {
            if (maxIndex.Text == "" || minIndex.Text == "")
            {
                return;
            }
            ShowOnlyMinMax();
        }

        private void maxIndex_TextChanged(object sender, EventArgs e)
        {
            if (maxIndex.Text == "" || minIndex.Text == "")
            {
                return;
            }
            ShowOnlyMinMax();
        }

        private void ShowOnlyMinMax()
        {
            int minIndexInt = Convert.ToInt32(minIndex.Text);
            int maxIndexInt = Convert.ToInt32(maxIndex.Text);
            var nodesToShow = instructionWrappers
                                .Where(x => x.InstructionIndex >= minIndexInt && x.InstructionIndex <= maxIndexInt)
                                .Select(x => GetNodeWrapper(x).Node)
                                .ToList();
            NodesToShow.Clear();
            NodesToShow.AddRange(nodesToShow);
            ReShow();
        }
    }
}
