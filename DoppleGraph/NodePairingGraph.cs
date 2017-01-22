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
        private Dictionary<LabeledVertex, List<LabeledVertex>> _pairings;
        CodeColorHanlder colorCode = new CodeColorHanlder();
        private GoLayer dataLinksLayer;
        private GoLayer flowAffectingLinksLayer;
        private GoLayer flowRoutesLinksLayer;

        public IEnumerable<GoLabeledVertexWrapper> AllNodeWrappers { get; private set; }

        public NodePairingGraph(Dictionary<LabeledVertex, List<LabeledVertex>> pairings)
        {
            _pairings = pairings;
            AllNodeWrappers =
                _pairings.Select(x => x.Key).Concat(_pairings.SelectMany(x => x.Value)).Select(x => new GoLabeledVertexWrapper(new GoTextNodeHoverable(), x)).ToList();
            InitializeComponent();
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

            

            var frontLayer = myView.Document.Layers.CreateNewLayerAfter(myView.Document.LinksLayer);
            foreach (var goNodeWrapper in AllNodeWrappers)
            {
                var shape = ((GoShape) goNodeWrapper.Node.Background);
                shape.BrushColor = colorCode.GetColor(goNodeWrapper.LabledVertex.Opcode);
                if (shape.BrushColor.GetBrightness() < 0.4)
                {
                    goNodeWrapper.Node.Label.TextColor = Color.White;
                }
                shape.Size = new SizeF(400, 400);

                goNodeWrapper.Node.Text = goNodeWrapper.LabledVertex.Opcode.ToString() + " index:" + goNodeWrapper.LabledVertex.Index;
                frontLayer.Add(goNodeWrapper.Node);
            }
            var smallGraphNodes = AllNodeWrappers.Where(x => x.LabledVertex.Method == _pairings.Keys.First().Method);
            var bigGraphNodes = AllNodeWrappers.Where(x => x.LabledVertex.Method == _pairings.Values.First(y => y.Count > 0).First().Method);
            SetCoordinates(smallGraphNodes, 0);
            SetCoordinates(bigGraphNodes, 1);
            DrawLinks(myView);
        }

        private void SetCoordinates(IEnumerable<GoLabeledVertexWrapper> nodeWrappers, int graphIndex)
        {
            SetLongestPath(nodeWrappers);
            foreach (var nodewrapper in nodeWrappers)
            {
                nodewrapper.DisplayCol = nodewrapper.LongestPath.Count;
            }
            Dictionary<int, List<GoNodeWrapper>> nodeWrapperCols = new Dictionary<int, List<GoNodeWrapper>>();
            foreach(var nodeColumn in nodeWrappers.GroupBy(x => x.LongestPath.Count))
            {
                int i = 0;
                foreach( var node in nodeColumn)
                {
                    node.DisplayRow = i;
                    node.DisplayColumn = nodeColumn.Key;
                    i++;
                }
            }
            int totalHeight = 1000;
            int totalWidth = 2000;
            int widthGraphOffset = totalWidth * graphIndex;
            float heightOffset = Convert.ToSingle(totalHeight / nodeWrappers.Select(x => x.DisplayRow).Max());
            float widthOffset = Convert.ToSingle(totalWidth / nodeWrappers.Select(x => x.DisplayCol).Max()) + widthGraphOffset;
            foreach (var nodeWrapper in nodeWrappers)
            {
                nodeWrapper.Node.Location = new PointF(nodeWrapper.DisplayCol * widthOffset, (nodeWrapper.DisplayRow - 0.7f) * heightOffset);
            }
        }

        private void DrawLinks(GoView myView)
        {
            dataLinksLayer = myView.Document.Layers.CreateNewLayerAfter(myView.Document.Layers.Default);
            flowAffectingLinksLayer = myView.Document.Layers.CreateNewLayerAfter(myView.Document.Layers.Default);
            flowRoutesLinksLayer = myView.Document.Layers.CreateNewLayerAfter(myView.Document.Layers.Default);
            foreach(var pair in _pairings)
            {
                foreach(var dest in pair.Value)
                {
                    LabeledEdge pairingEdge = new LabeledEdge();
                    pairingEdge.SourceVertex = pair.Key;
                    pairingEdge.DestinationVertex = dest;
                    pairingEdge.EdgeType = EdgeType.Pairing;
                    pair.Key.ForwardEdges.Add(pairingEdge);
                    dest.BackEdges.Add(pairingEdge);
                }
            }
            foreach (var edge in AllNodeWrappers.SelectMany(x => x.LabledVertex.BackEdges.Concat(x.LabledVertex.ForwardEdges)))
            {
                DrawEdge(edge);
            }
        }

        private void DrawEdge(LabeledEdge edge)
        {
            Color edgeColor;
            if (edge.EdgeType == EdgeType.Pairing)
            {
                edgeColor = Color.LightBlue;
            }
            else
            {
                edgeColor = Color.LightGray;
            }
            GoLink link = new GoLink();
            link.Pen = new Pen(edgeColor);
            var destinationVertexWrapper = GetWrapper(edge.DestinationVertex);
            var sourceVertexWrapper = GetWrapper(edge.SourceVertex);
            if (destinationVertexWrapper == null || sourceVertexWrapper == null)
            {
                return;
            }
            link.ToPort = GetWrapper(edge.DestinationVertex).Node.LeftPort;
            link.FromPort = GetWrapper(edge.SourceVertex).Node.RightPort;
            if (edge.DestinationVertex == edge.SourceVertex)
            {
                link.Style = GoStrokeStyle.Bezier;
                link.CalculateRoute();
                foreach (int index in new[] { 1, 2 })
                {
                    link.SetPoint(index, new PointF(link.GetPoint(index).X, link.GetPoint(index).Y - 40));
                }
            }
            dataLinksLayer.Add(link);
            link.PenWidth = 3;
        }

        private void SetLongestPath(IEnumerable<GoLabeledVertexWrapper> vertexesToSet)
        {
            var firstVertex = vertexesToSet.First(x => x.LabledVertex.BackEdges.Count == 0);
            Queue<GoLabeledVertexWrapper> vertexesToResolve = new Queue<GoLabeledVertexWrapper>();
            vertexesToResolve.Enqueue(firstVertex);
            firstVertex.LongestPath.Add(firstVertex);
            var visited = new List<GoLabeledVertexWrapper>();
            while (vertexesToResolve.Count != 0)
            {
                GoLabeledVertexWrapper currentVertex = vertexesToResolve.Dequeue();
                foreach(var forwardVertex in currentVertex.LabledVertex.ForwardEdges.Where(x => x.EdgeType == EdgeType.DataFlow))
                {
                    var forwardVertexWrapper = GetWrapper(forwardVertex.DestinationVertex);
                    if (forwardVertexWrapper == null)
                    {
                        continue;
                    }
                    if (visited.Contains(forwardVertexWrapper))
                    {

                    }
                    else
                    {
                        visited.Add(forwardVertexWrapper);
                    }
                    if (!currentVertex.LongestPath.Contains(forwardVertexWrapper) && !forwardVertexWrapper.LongestPath.Contains(currentVertex) && currentVertex.LongestPath.Count + 1 > forwardVertexWrapper.LongestPath.Count)
                    {
                        forwardVertexWrapper.LongestPath = new List<GoLabeledVertexWrapper>(currentVertex.LongestPath);
                        forwardVertexWrapper.LongestPath.Add(forwardVertexWrapper);
                        vertexesToResolve.Enqueue(forwardVertexWrapper);
                    } 
                }
            }
        }

        private GoLabeledVertexWrapper GetWrapper(LabeledVertex vertex)
        {
            return AllNodeWrappers.FirstOrDefault(x => x.LabledVertex == vertex);
        }
    }
}
