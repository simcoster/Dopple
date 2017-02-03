using GraphSimilarityByMatching;
using Northwoods.Go;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoppleGraph
{
    public partial class NodePairingGraph : Form
    {
        private GoView myView;
        private NodePairings _pairings;
        CodeColorHanlder colorCode = new CodeColorHanlder();
        private GoLayer dataLinksLayer;
        private GoLayer flowAffectingLinksLayer;
        private GoLayer flowRoutesLinksLayer;
        private IEnumerable<GoLabeledVertexWrapper> SmallGraphNodes;
        private IEnumerable<GoLabeledVertexWrapper> BigGraphNodes;

        public IEnumerable<GoLabeledVertexWrapper> AllNodeWrappers { get; private set; }

        public NodePairingGraph(NodePairings pairings, double score)
        {
            _pairings = pairings ;
            SmallGraphNodes = pairings.SecondGraph.Select(x => new GoLabeledVertexWrapper(new GoTextNodeHoverable(), x)).ToList();
            BigGraphNodes = pairings.FirstGraph.Select(x => new GoLabeledVertexWrapper(new GoTextNodeHoverable(), x)).ToList();
            AllNodeWrappers = BigGraphNodes.Concat(SmallGraphNodes).ToList();
            InitializeComponent();
            ScoreLbl.Text = score.ToString();
        }

        private void NodePairingGraph_Load(object sender, EventArgs e)
        {
            // create a Go view (a Control) and add to the form
            myView = new GoView();
            myView.AllowDelete = false;
            myView.AllowInsert = false;
            myView.AllowLink = false;
            myView.Dock = DockStyle.Fill;
            this.Controls.Add(myView);
            myView.Show();
            BigGraphMethodLbl.Text = BigGraphNodes.First().LabledVertex.Method.Name;
            SmallGraphMethodlbl.Text = SmallGraphNodes.First().LabledVertex.Method.Name;

            var frontLayer = myView.Document.Layers.CreateNewLayerAfter(myView.Document.LinksLayer);
            foreach (var goNodeWrapper in AllNodeWrappers)
            {
                var shape = ((GoShape) goNodeWrapper.Node.Background);
                shape.BrushColor = colorCode.GetColor(goNodeWrapper.LabledVertex.Opcode);
                shape.PenWidth = 3;
                shape.PenColor = SmallGraphNodes.Contains(goNodeWrapper) ? Color.Green : Color.Orange;
                if (shape.BrushColor.GetBrightness() < 0.4)
                {
                    goNodeWrapper.Node.Label.TextColor = Color.White;
                }
                shape.Size = new SizeF(400, 400);

                goNodeWrapper.Node.Text = goNodeWrapper.LabledVertex.Opcode.ToString() + " index:" + goNodeWrapper.LabledVertex.Index;
                goNodeWrapper.Node.Selected += Node_Selected;
                goNodeWrapper.Node.UnSelected += Node_UnSelected;
                frontLayer.Add(goNodeWrapper.Node);
            }
            DrawLinks(myView);
            SetCoordinates();
        }

        private void Node_UnSelected(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Node_Selected(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void SetCoordinates()
        {
            var smallNodeWrappersClone = new List<GoLabeledVertexWrapper>(SmallGraphNodes);
            var bigNodeWrappersClone = new List<GoLabeledVertexWrapper>(BigGraphNodes);
            int displayCol = 0;
            while (smallNodeWrappersClone.Count > 0)
            {
                for (int i =0;i<20 && smallNodeWrappersClone.Count > i ; i++)
                {
                    var currSmallNode = smallNodeWrappersClone[i];
                    currSmallNode.DisplayCol = displayCol;
                    currSmallNode.DisplayRow = i;
                    var pairedVertices = currSmallNode.LabledVertex.PairingEdges.Select(x => x.BigGraphVertex).ToList();
                    foreach (var bigGraphNode in pairedVertices)
                    {
                        var bigGraphNodeWrapper = BigGraphNodes.First(x => x.LabledVertex == bigGraphNode);
                        bigGraphNodeWrapper.DisplayRow = i + 1/((float)pairedVertices.IndexOf(bigGraphNode) +1);
                        bigGraphNodeWrapper.DisplayCol = displayCol + 1;
                        bigNodeWrappersClone.Remove(bigGraphNodeWrapper);
                    }
                    smallNodeWrappersClone.Remove(currSmallNode);
                }
                displayCol += 2;
            }
            while (bigNodeWrappersClone.Count > 0)
            {
                for (int i = 0; i < 20 && bigNodeWrappersClone.Count > i ; i++)
                {
                    var currBigNode = bigNodeWrappersClone[i];
                    currBigNode.DisplayCol = displayCol;
                    currBigNode.DisplayRow = i;
                    bigNodeWrappersClone.Remove(currBigNode);
                }
                displayCol += 1;
            }
            int totalHeight = 1000;
            int totalWidth = 2000;
            float heightOffset = Convert.ToSingle(totalHeight / AllNodeWrappers.Select(x => x.DisplayRow).Max());
            float widthOffset = Convert.ToSingle(totalWidth / AllNodeWrappers.Select(x => x.DisplayCol).Max()) ;
            foreach (var nodeWrapper in AllNodeWrappers)
            {
                nodeWrapper.Node.Location = new PointF(nodeWrapper.DisplayCol * widthOffset , (nodeWrapper.DisplayRow - 0.7f) * heightOffset );
            }
        }

        private void DrawLinks(GoView myView)
        {
            dataLinksLayer = myView.Document.Layers.CreateNewLayerAfter(myView.Document.Layers.Default);
            flowAffectingLinksLayer = myView.Document.Layers.CreateNewLayerAfter(myView.Document.Layers.Default);
            flowRoutesLinksLayer = myView.Document.Layers.CreateNewLayerAfter(myView.Document.Layers.Default);
            foreach(var pair in _pairings.Pairings)
            {
                foreach(var bigGraphVertex in pair.Value)
                {
                    SmallBigLinkEdge pairingEdge = new SmallBigLinkEdge();
                    pairingEdge.BigGraphVertex = pair.Key;
                    pairingEdge.SmallGraphVertex = bigGraphVertex.PairedVertex;
                    pairingEdge.Score = bigGraphVertex.PairingScore;
                    pair.Key.PairingEdges.Add(pairingEdge);
                    bigGraphVertex.PairedVertex.PairingEdges.Add(pairingEdge);
                }
            }
            foreach(var pairinigEdge in SmallGraphNodes.SelectMany(x => x.LabledVertex.PairingEdges))
            {
                DrawPairingEdge(pairinigEdge);
            }
        }

        private void DrawPairingEdge(SmallBigLinkEdge pairinigEdge)
        {
            GoLink link = new GoLink();
            Color edgeColor = Color.FromArgb(Convert.ToInt32(pairinigEdge.Score * 2), 0, 255 - Convert.ToInt32(pairinigEdge.Score * 2));
            link.ToolTipText = pairinigEdge.Score.ToString();
            var smallVertexWrapper = SmallGraphNodes.First(x => x.LabledVertex == pairinigEdge.SmallGraphVertex);
            var bigVertexWrapper = BigGraphNodes.First(x => x.LabledVertex == pairinigEdge.BigGraphVertex);
            link.Pen = new Pen(edgeColor);
            if (bigVertexWrapper == null || smallVertexWrapper == null)
            {
                return;
            }
            link.ToPort = bigVertexWrapper.Node.LeftPort;
            link.FromPort = smallVertexWrapper.Node.RightPort;
            dataLinksLayer.Add(link);
            link.PenWidth = 3;
        }

        

        //private void SetLongestPath(IEnumerable<GoLabeledVertexWrapper> vertexesToSet)
        //{
        //    var firstVertex = vertexesToSet.First(x => x.LabledVertex.BackEdges.Count == 0);
        //    Queue<GoLabeledVertexWrapper> vertexesToResolve = new Queue<GoLabeledVertexWrapper>();
        //    vertexesToResolve.Enqueue(firstVertex);
        //    firstVertex.LongestPath.Add(firstVertex);
        //    var visited = new List<GoLabeledVertexWrapper>();
        //    while (vertexesToResolve.Count != 0)
        //    {
        //        GoLabeledVertexWrapper currentVertex = vertexesToResolve.Dequeue();
        //        foreach(var forwardVertex in currentVertex.LabledVertex.ForwardEdges.Where(x => x.EdgeType == EdgeType.DataFlow))
        //        {
        //            var forwardVertexWrapper = GetWrapper(forwardVertex.DestinationVertex);
        //            if (forwardVertexWrapper == null)
        //            {
        //                continue;
        //            }
        //            if (visited.Contains(forwardVertexWrapper))
        //            {

        //            }
        //            else
        //            {
        //                visited.Add(forwardVertexWrapper);
        //            }
        //            if (!currentVertex.LongestPath.Contains(forwardVertexWrapper) && !forwardVertexWrapper.LongestPath.Contains(currentVertex) && currentVertex.LongestPath.Count + 1 > forwardVertexWrapper.LongestPath.Count)
        //            {
        //                forwardVertexWrapper.LongestPath = new List<GoLabeledVertexWrapper>(currentVertex.LongestPath);
        //                forwardVertexWrapper.LongestPath.Add(forwardVertexWrapper);
        //                vertexesToResolve.Enqueue(forwardVertexWrapper);
        //            } 
        //        }
        //    }
        //}

        private GoLabeledVertexWrapper GetWrapper(LabeledVertex vertex)
        {
            return AllNodeWrappers.FirstOrDefault(x => x.LabledVertex == vertex);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
