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
        private IEnumerable<GoLabeledVertexWrapper> FirstGraphNodes;
        private IEnumerable<GoLabeledVertexWrapper> SecondGraphNodes;

        public IEnumerable<GoLabeledVertexWrapper> AllNodeWrappers { get; private set; }

        public NodePairingGraph(NodePairings pairings, double score)
        {
            _pairings = pairings ;
            FirstGraphNodes = pairings.SecondGraph.Select(x => new GoLabeledVertexWrapper(new GoTextNodeHoverable(), x)).ToList();
            SecondGraphNodes = pairings.FirstGraph.Select(x => new GoLabeledVertexWrapper(new GoTextNodeHoverable(), x)).ToList();
            AllNodeWrappers = SecondGraphNodes.Concat(FirstGraphNodes).ToList();
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
            SecondGraphMethodLbl.Text = SecondGraphNodes.First().LabledVertex.Method.Name;
            FirstGraphMethodlbl.Text = FirstGraphNodes.First().LabledVertex.Method.Name;

            var frontLayer = myView.Document.Layers.CreateNewLayerAfter(myView.Document.LinksLayer);
            foreach (var goNodeWrapper in AllNodeWrappers)
            {
                var shape = ((GoShape) goNodeWrapper.Node.Background);
                shape.BrushColor = colorCode.GetColor(goNodeWrapper.LabledVertex.Opcode);
                shape.PenWidth = 3;
                shape.PenColor = FirstGraphNodes.Contains(goNodeWrapper) ? Color.Green : Color.Orange;
                if (shape.BrushColor.GetBrightness() < 0.4)
                {
                    goNodeWrapper.Node.Label.TextColor = Color.White;
                }
                shape.Size = new SizeF(400, 400);

                goNodeWrapper.Node.Text = goNodeWrapper.LabledVertex.Opcode.ToString() + " index:" + goNodeWrapper.LabledVertex.Index;
                if (goNodeWrapper.LabledVertex is MultiNodeLabeledVertex)
                {
                    goNodeWrapper.Node.Text += "multi";
                }
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
            var firstNodeWrappersClone = new List<GoLabeledVertexWrapper>(FirstGraphNodes);
            var secondNodeWrappersClone = new List<GoLabeledVertexWrapper>(SecondGraphNodes);
            int displayCol = 0;
            while (firstNodeWrappersClone.Count > 0)
            {
                for (int i =0;i<20 && firstNodeWrappersClone.Count > i ; i++)
                {
                    var currSmallNode = firstNodeWrappersClone[i];
                    currSmallNode.DisplayCol = displayCol;
                    currSmallNode.DisplayRow = i;
                    var pairedVertices = currSmallNode.LabledVertex.PairingEdges.Select(x => x.SecondGraphVertex).ToList();
                    foreach (var bigGraphNode in pairedVertices)
                    {
                        var bigGraphNodeWrapper = SecondGraphNodes.First(x => x.LabledVertex == bigGraphNode);
                        bigGraphNodeWrapper.DisplayRow = i + 1/((float)pairedVertices.IndexOf(bigGraphNode) +1);
                        bigGraphNodeWrapper.DisplayCol = displayCol + 1;
                        secondNodeWrappersClone.Remove(bigGraphNodeWrapper);
                    }
                    firstNodeWrappersClone.Remove(currSmallNode);
                }
                displayCol += 2;
            }
            while (secondNodeWrappersClone.Count > 0)
            {
                for (int i = 0; i < 20 && secondNodeWrappersClone.Count > i ; i++)
                {
                    var currBigNode = secondNodeWrappersClone[i];
                    currBigNode.DisplayCol = displayCol;
                    currBigNode.DisplayRow = i;
                    secondNodeWrappersClone.Remove(currBigNode);
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
            foreach(var firstGraphNodePairing in _pairings.Pairings)
            {
                foreach(var secondGraphNode in firstGraphNodePairing.Value)
                {
                    SmallBigLinkEdge pairingEdge = new SmallBigLinkEdge();
                    pairingEdge.SecondGraphVertex = secondGraphNode.PairedVertex;
                    pairingEdge.FirstGraphVertex = firstGraphNodePairing.Key;
                    pairingEdge.Score = secondGraphNode.PairingScore;
                    firstGraphNodePairing.Key.PairingEdges.Add(pairingEdge);
                    secondGraphNode.PairedVertex.PairingEdges.Add(pairingEdge);
                }
            }
            foreach(var pairingEdge in FirstGraphNodes.SelectMany(x => x.LabledVertex.PairingEdges))
            {
                DrawPairingEdge(pairingEdge);
            }
        }

        private void DrawPairingEdge(SmallBigLinkEdge pairinigEdge)
        {
            GoLink link = new GoLink();
            //Color edgeColor = Color.FromArgb(Convert.ToInt32(pairinigEdge.Score), 0, 255 - Convert.ToInt32(pairinigEdge.Score));
            Color edgeColor = Color.Blue;
            link.ToolTipText = pairinigEdge.Score.ToString();
            var firstVertexWrapper = FirstGraphNodes.First(x => x.LabledVertex == pairinigEdge.FirstGraphVertex);
            var secondVertexWrapper = SecondGraphNodes.First(x => x.LabledVertex == pairinigEdge.SecondGraphVertex);
            link.Pen = new Pen(edgeColor);
            if (secondVertexWrapper == null || firstVertexWrapper == null)
            {
                return;
            }
            link.ToPort = secondVertexWrapper.Node.LeftPort;
            link.FromPort = firstVertexWrapper.Node.RightPort;
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
