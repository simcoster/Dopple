using Dopple;
using Dopple.InstructionNodes;
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
        GoLayer flowRoutesLinksLayer;
        GoLayer dataLinksLayer;
        GoLayer flowAffectingLinksLayer;

        private Form2()
        {
            InitializeComponent();
        }

        public Form2(List<InstructionNode> instructionNodes)
        {
            InstructionNodes = instructionNodes;
            InitializeComponent();
        }

        const int NodeSelectedHideIndex = 1;
        const int FlowRouteLinkHideIndex = 2;
        const int FlowAffectingLinkHideIndex = 3;
        const int DataLinkHideIndex = 4;
        const int DataBackTreeHideIndex = 5;
        const int FlowBackTreeHideIndex = 6;
        const int HiddenNodesHideIndex = 7;
        const int RightLinksHideIndex = 8;
        const int LeftLinksHideIndex = 9;


        private Dictionary<int, List<GoObject>> ObjectsToHide = new Dictionary<int, List<GoObject>>() {
                                                                                    { NodeSelectedHideIndex,new List<GoObject>() },
                                                                                    { FlowRouteLinkHideIndex,new List<GoObject>() },
                                                                                    { FlowAffectingLinkHideIndex,new List<GoObject>() },
                                                                                    { DataLinkHideIndex,new List<GoObject>() },
                                                                                    { DataBackTreeHideIndex,new List<GoObject>() },
                                                                                    { FlowBackTreeHideIndex,new List<GoObject>() },
                                                                                    { HiddenNodesHideIndex,new List<GoObject>() } ,
                                                                                    { RightLinksHideIndex,new List<GoObject>() },
                                                                                    { LeftLinksHideIndex,new List<GoObject>() } };
        private Random rnd = new Random();
        private List<InstructionNode> InstructionNodes;

        private void Node_UnSelected(object sender, EventArgs e)
        {
            lock (ObjectsToHide)
            {
                if (myView.Selection.Count == 0)
                {
                    ObjectsToHide[NodeSelectedHideIndex].Clear();
                }              
                ReShow();
            }

        }

        private void Node_Selected(object sender, EventArgs e)
        {
            lock(ObjectsToHide)
            {
                ObjectsToHide[NodeSelectedHideIndex].Clear();

                if (myView.Selection.Count >1)
                {
                    ObjectsToHide[NodeSelectedHideIndex].AddRange(GetObjectsToHide(myView.Selection.Where(x => x is GoNode).Cast<GoNode>()));
                }
                else if (sender is GoNode)
                {
                    ObjectsToHide[NodeSelectedHideIndex].AddRange(GetObjectsToHide(new GoNode[] { (GoNode) sender }));
                }
                ReShow();
            }
        }

        private void ReShow()
        {
            IEnumerable<GoObject> objectsToHide = ObjectsToHide.SelectMany(x => x.Value);
            var objectsToShow = myView.Document.Except(objectsToHide);
            foreach (var goObject in objectsToHide)
            {
                goObject.Visible = false;
            }
            foreach (var goObject in objectsToShow)
            {
                goObject.Visible = true;
            }
        }


        private void MyView_KeyPress(object sender, KeyPressEventArgs e)
        {
            var backTraceManager = new GraphBuilder(InstructionNodes);
            if (e.KeyChar == '/')
            {
                ObjectsToHide[HiddenNodesHideIndex].AddRange(myView.Selection);
                ObjectsToHide[HiddenNodesHideIndex].AddRange(myView.Selection.Where(x => x is GoNode).Cast<GoNode>().SelectMany(x => x.Links).Cast<GoLink>());
            }
            else if (e.KeyChar == '.')
            {
                if (myView.Selection.Count > 0)
                {
                    ObjectsToHide[DataBackTreeHideIndex].Clear();
                    var backTreeNodes = BackSearcher.GetBackDataTree(GetNodeWrapper(myView.Selection.First(x => x is GoNode) as GoNode).InstructionNode)
                                                                                                          .Select(x => GetNodeWrapper(x).Node).ToList();
                    ObjectsToHide[DataBackTreeHideIndex].AddRange(GetObjectsToHide(backTreeNodes));
                }                
            }
            else if (e.KeyChar == '+')
            {
                if (myView.Selection.Count > 0)
                {
                    ObjectsToHide[DataBackTreeHideIndex].Clear();
                    var backTreeNodes = BackSearcher.GetBackFlowTree(GetNodeWrapper(myView.Selection.First(x => x is GoNode) as GoNode).InstructionNode)
                                                                                                          .Select(x => GetNodeWrapper(x).Node).ToList();
                    ObjectsToHide[DataBackTreeHideIndex].AddRange(GetObjectsToHide(backTreeNodes));
                }
            }
            else if (e.KeyChar == '*')
            {
                ObjectsToHide.ForEach(x => ObjectsToHide[x.Key].Clear());
            }
            ReShow();
        }

        IEnumerable<GoObject> GetObjectsToHide(IEnumerable<GoNode> nodesToShow)
        {
            var tempNodesToShow = new List<GoObject>(nodesToShow);
            tempNodesToShow.AddRange(nodesToShow.SelectMany(x => x.Links.Select(y => y.FromNode)).Cast<GoNode>());
            tempNodesToShow.AddRange(nodesToShow.SelectMany(x => x.Links.Select(y => y.ToNode)).Cast<GoNode>());
            tempNodesToShow.AddRange(nodesToShow.SelectMany(x => x.Links).Cast<GoLink>());
            tempNodesToShow = tempNodesToShow.Distinct().ToList();
            return myView.Document.Except(tempNodesToShow);
        }

        private void ShowFlowLinks_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowProgramFlowLinks.Checked)
            {
                ObjectsToHide[FlowRouteLinkHideIndex].Clear();
            }
            else
            {
                ObjectsToHide[FlowRouteLinkHideIndex].AddRange(flowRoutesLinksLayer);
            }
            ReShow();
        }

        private void ShowDataFlowLinks_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowDataFlowLinks.Checked)
            {
                ObjectsToHide[DataLinkHideIndex].Clear();
            }
            else
            {
                ObjectsToHide[DataLinkHideIndex].AddRange(dataLinksLayer);
            }
            ReShow();

        }


        private void ShowFlowAffectingLinks_CheckedChanged(object sender, EventArgs e)
        {
            if (FlowAffectingChb.Checked)
            {
                ObjectsToHide[FlowAffectingLinkHideIndex].Clear();
            }
            else
            {
                ObjectsToHide[FlowAffectingLinkHideIndex].AddRange(flowAffectingLinksLayer);
            }
            ReShow();

        }

        private void ShowRightDataLinks_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowRightDataLinks.Checked)
            {
                ObjectsToHide[RightLinksHideIndex].Clear();
            }
            else
            {
                IEnumerable<GoLink> leftLinksToHide = myView.Selection.Where(x => x is GoTextNode).Cast<GoTextNode>().SelectMany(x => x.LeftPort.Links).Cast<GoLink>();
                ObjectsToHide[RightLinksHideIndex].AddRange(leftLinksToHide);
            }
            ReShow();

        }

        private void ShowLeftDataLinks_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowLeftDataLinks.Checked)
            {
                ObjectsToHide[LeftLinksHideIndex].Clear();
            }
            else
            {
                IEnumerable<GoLink> rightLinksToHide = myView.Selection.Where(x => x is GoTextNode).Cast<GoTextNode>().SelectMany(x => x.RightPort.Links).Cast<GoLink>();
                ObjectsToHide[LeftLinksHideIndex].AddRange(rightLinksToHide);
            }
            ReShow();

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
            if (maxIndexInt <= minIndexInt)
            {
                return;
            }
            var nodesToShow = InstructionNodes
                                .Where(x => x.InstructionIndex >= minIndexInt && x.InstructionIndex <= maxIndexInt)
                                .Select(x => GetNodeWrapper(x).Node)
                                .ToList();
            ObjectsToHide[HiddenNodesHideIndex].Clear();
            ObjectsToHide[HiddenNodesHideIndex].AddRange(GetObjectsToHide(nodesToShow));
            ReShow();
        }

        private void DrawFlowLinks(GoNodeWrapper nodeWrapper, GoView myView)
        {
            foreach (InstructionNode wrapper in nodeWrapper.InstructionNode.ProgramFlowForwardRoutes)
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

        public void DrawLinks(GoView myView)
        {
            dataLinksLayer = myView.Document.Layers.CreateNewLayerAfter(myView.Document.Layers.Default);
            flowAffectingLinksLayer = myView.Document.Layers.CreateNewLayerAfter(myView.Document.Layers.Default);
            flowRoutesLinksLayer = myView.Document.Layers.CreateNewLayerAfter(myView.Document.Layers.Default);
            foreach (var nodeWrapper in nodeWrappers)
            {
                foreach (var indexedArg in nodeWrapper.InstructionNode.DataFlowBackRelated)
                {
                    try
                    {
                        Color linkColor = GetPredefinedDataLinkColor(indexedArg.ArgIndex);
                        GoLink edge = DrawEdge(nodeWrapper, myView, indexedArg.Argument, dataLinksLayer, new Pen(linkColor));
                        edge.ToolTipText = indexedArg.ArgIndex.ToString();
                    }
                    catch
                    {
                        Console.WriteLine("Failed to draw edge");
                    }
                }
                foreach (var indexedArg in nodeWrapper.InstructionNode.ProgramFlowBackAffected)
                {
                    try
                    {
                        Color linkColor = GetPredefinedFlowAffectLinkColor(indexedArg.ArgIndex);
                        GoLink edge = DrawEdge(nodeWrapper, myView, indexedArg.Argument, flowAffectingLinksLayer, new Pen(linkColor));
                        edge.ToolTipText = indexedArg.ArgIndex.ToString() + " " + edge.PenColor.R;
                    }
                    catch
                    {
                        Console.WriteLine("Failed to draw edge");
                    }
                }
                foreach (var backRouteNode in nodeWrapper.InstructionNode.ProgramFlowBackRoutes)
                {
                    try
                    {
                        DrawEdge(nodeWrapper, myView, backRouteNode, flowRoutesLinksLayer, new Pen(Color.Black) { DashStyle = DashStyle.Dash });
                    }
                    catch
                    {
                        Console.WriteLine("Failed to draw edge");
                    }
                }
                foreach (var backRelatedNode in nodeWrapper.InstructionNode.SingleUnitBackRelated)
                {
                    try
                    {
                        DrawEdge(nodeWrapper, myView, backRelatedNode, flowRoutesLinksLayer, new Pen(Color.Blue) { DashStyle = DashStyle.Dot });
                    }
                    catch
                    { }
                }
            }
        }

        private GoLink DrawEdge(GoNodeWrapper nodeWrapper, GoView myView, InstructionNode backNode, GoLayer layer, Pen pen)
        {
            GoLink link = new GoLink();
            var backNodeWrapper = GetNodeWrapper(backNode);
            link.Pen = pen;
            link.ToPort = nodeWrapper.Node.LeftPort;
            if (backNodeWrapper.Node == nodeWrapper.Node)
            {
                link.FromPort = backNodeWrapper.Node.RightPort;
                link.Style = GoStrokeStyle.Bezier;
                link.CalculateRoute();
                foreach (int index in new[] { 1, 2 })
                {
                    link.SetPoint(index, new PointF(link.GetPoint(index).X, link.GetPoint(index).Y - 40));
                }
            }
            else
            {
                link.FromPort = backNodeWrapper.Node.RightPort;
            }
            layer.Add(link);
            link.PenWidth = 3;
            return link;
        }

        private Color GetPredefinedDataLinkColor(int argIndex)
        {
            switch (argIndex)
            {
                case 0:
                    return Color.Blue;
                case 1:
                    return Color.Green;
                case 2:
                    return Color.Orange;
                case 3:
                    return Color.Purple;
                case 4:
                    return Color.Brown;
                default:
                    return Color.Red;
            }
        }

        private Color GetPredefinedFlowAffectLinkColor(int argIndex)
        {
            switch (argIndex)
            {
                case 1:
                    return Color.Pink;
                case 2:
                    return Color.Yellow;
                case 3:
                    return Color.LightSeaGreen;

                default:
                    throw new Exception("no one here");
            }
        }

        private void SetCoordinates(List<GoNodeWrapper> nodeWrappers)
        {
            Dictionary<int, List<GoNodeWrapper>> nodeWrapperCols = new Dictionary<int, List<GoNodeWrapper>>();
            var firstNode = nodeWrappers.Where(x => x.InstructionNode.DataFlowBackRelated.Count == 0).ToList();
            SetLongestPathRec(firstNode);
            SetRowIndexes(nodeWrappers);
            FixDuplicateCoordinates(nodeWrappers);
            int totalHeight = 1000;
            int totalWidth = 8000;
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

        private GoNodeWrapper GetNodeWrapper(InstructionNode instWrapper)
        {
            return nodeWrappers.FirstOrDefault(x => x.InstructionNode == instWrapper);
        }

        private GoNodeWrapper GetNodeWrapper(GoNode goNode)
        {
            return nodeWrappers.First(x => x.Node == goNode);
        }

        private void SetLongestPathRec(List<GoNodeWrapper> colNodes)
        {
            foreach (var node in colNodes)
            {
                //var nodesToUpdate = node.InstructionNode.DataFlowForwardRelated
                var nodesToUpdate = node.InstructionNode.ProgramFlowForwardRoutes
               //.Select(x => GetNodeWrapper(x.Argument))
               .Select(x => GetNodeWrapper(x))
               //TODO remove, this hides a problem
               .Where(x=> x != null)
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
        }

        private void SetRowIndexes(List<GoNodeWrapper> allNodes)
        {
            Dictionary<MethodDefinition, int> methodRows = new Dictionary<MethodDefinition, int>();
            int highestRowIndex = 1;
            foreach (var node in allNodes)
            {
                if (!methodRows.ContainsKey(node.InstructionNode.Method))
                {
                    methodRows.Add(node.InstructionNode.Method, highestRowIndex);
                    highestRowIndex++;
                }
                node.DisplayRow = methodRows[node.InstructionNode.Method];
            }
            return;
            foreach (int col in allNodes.Select(x => x.DisplayCol).Distinct())
            {
                var orderedNodes = allNodes.Where(x => x.DisplayCol == col)
                    .OrderBy(x => x.InstructionNode.Instruction.OpCode.Code.ToString()).ToList();
                foreach (var node in orderedNodes)
                {
                    node.DisplayRow = orderedNodes.IndexOf(node) +1;
                }
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Text = InstructionNodes[0].Method.Name;
            // create a Go view (a Control) and add to the form
            myView = new GoView();
            myView.KeyPress += MyView_KeyPress;
            myView.SelectionFinished += Node_Selected;
            myView.AllowDelete = false;
            myView.AllowInsert = false;
            myView.AllowLink = false;
            myView.Dock = DockStyle.Fill;
            this.Controls.Add(myView);
            myView.Show();

            nodeWrappers =
                InstructionNodes
                .Select(x => new GoNodeWrapper(new GoTextNodeHoverable(), x))
                .ToList();

            var frontLayer = myView.Document.Layers.CreateNewLayerAfter(myView.Document.LinksLayer);
            foreach (var goNodeWrapper in nodeWrappers)
            {
                goNodeWrapper.Node.Selected += Node_Selected;
                goNodeWrapper.Node.UnSelected += Node_UnSelected;
                goNodeWrapper.Index = nodeWrappers.IndexOf(goNodeWrapper);
                var shape = ((GoShape) goNodeWrapper.Node.Background);
                shape.BrushColor = colorCode.GetColor(goNodeWrapper.InstructionNode.Instruction.OpCode.Code);
                if (shape.BrushColor.GetBrightness() < 0.4)
                {
                    goNodeWrapper.Node.Label.TextColor = Color.White;
                }
                shape.Size = new SizeF(400, 400);

                if (goNodeWrapper.InstructionNode.InliningProperties.Inlined || goNodeWrapper.InstructionNode.MarkForDebugging)
                {
                    if (goNodeWrapper.InstructionNode.SingleUnitNodes.Count > 0 || goNodeWrapper.InstructionNode.StackPopCount > 0)
                    {
                        goNodeWrapper.Node.Shape.PenColor = Color.Red;
                    }
                    else
                    {
                        goNodeWrapper.Node.Shape.PenColor = Color.Blue;
                    }
                    goNodeWrapper.Node.Shape.PenWidth = 3;
                    goNodeWrapper.Node.ToolTipText = goNodeWrapper.InstructionNode.Method.FullName + "***";
                }

                goNodeWrapper.Node.Text = goNodeWrapper.InstructionNode.Instruction.OpCode.Code.ToString() + " index:" + goNodeWrapper.InstructionNode.InstructionIndex + " offset:" + goNodeWrapper.InstructionNode.Instruction.Offset + " ";

                if (new[] { Code.Call, Code.Calli, Code.Callvirt }.Contains(
                        goNodeWrapper.InstructionNode.Instruction.OpCode.Code))
                {
                    goNodeWrapper.Node.Text += ((MethodReference) goNodeWrapper.InstructionNode.Instruction.Operand).FullName ?? " ";
                }
                else if (goNodeWrapper.InstructionNode is FunctionArgInstNode)
                {
                    var ArgInstWrapper = (FunctionArgInstNode) goNodeWrapper.InstructionNode;
                    goNodeWrapper.Node.Text += " " + ArgInstWrapper.ArgName + " " + ArgInstWrapper.ArgIndex;
                }
                else if (goNodeWrapper.InstructionNode.Instruction.Operand != null)
                {
                    goNodeWrapper.Node.Text += goNodeWrapper.InstructionNode.Instruction.Operand.ToString();
                }

                if (goNodeWrapper.InstructionNode.InliningProperties.Recursive)
                {
                    goNodeWrapper.Node.Text += " RecIndex:" + goNodeWrapper.InstructionNode.InliningProperties.SameMethodCallIndex;
                    goNodeWrapper.Node.Text += " Recursive:" + goNodeWrapper.InstructionNode.InliningProperties.RecursionLevel;
                }
                frontLayer.Add(goNodeWrapper.Node);
            }
            SetCoordinates(nodeWrappers);
            DrawLinks(myView);
        }
    }
}
