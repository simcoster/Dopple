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
        private NodePairings _sourceGraphSelfPairings;
        CodeColorHanlder colorCode = new CodeColorHanlder();
        private IEnumerable<GoLabeledVertexWrapper> ImageGraphNodes;

        public IEnumerable<GoLabeledVertexWrapper> AllNodeWrappers { get; private set; }

        public NodePairingGraph(NodePairings pairings)
        {
            _sourceGraphSelfPairings = GraphSimilarityCalc.GetGraphSelfScore(pairings.SourceGraph);
            _pairings = pairings;
            ImageGraphNodes = pairings.Pairings.Keys.Select(x => new GoLabeledVertexWrapper(new GoTextNodeHoverable(), x)).ToList();
            InitializeComponent();
            ScoreLbl.Text = (pairings.TotalScore / _sourceGraphSelfPairings.TotalScore).ToString();
            var pairingValues = pairings.Pairings.Values.SelectMany(x => x);
            var selfPairingValues = _sourceGraphSelfPairings.Pairings.SelectMany(x => x.Value);
            var moreThanSelf = pairingValues.Select(x => new { Regular =  x.Score, Self =  selfPairingValues.First(y => y.SourceGraphVertex== x.SourceGraphVertex).Score }).ToList();
        }

        private static int ColumnOffset = 150;
        private static int RowOffset = 50;
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
            SourceGraphMethodLbl.Text = _pairings.SourceGraph.First().Method.Name;
            ImageGraphMethodlbl.Text = _pairings.ImageGraph.First().Method.Name;

            var frontLayer = myView.Document.Layers.CreateNewLayerAfter(myView.Document.LinksLayer);
            int column = 0;
            int row = 0;
            foreach (var imageNode in ImageGraphNodes)
            {
                if (row >100)
                {
                    row = 0;
                    column +=2;
                }
                SetShape(frontLayer, imageNode);
                frontLayer.Add(imageNode.Node);
                imageNode.Node.Location = new PointF(column * ColumnOffset, row * RowOffset);
                foreach(var sourceNodePairing in _pairings.Pairings[imageNode.LabledVertex])
                {
                    var sourceLabledVertex = new GoLabeledVertexWrapper(new GoTextNodeHoverable(), sourceNodePairing.SourceGraphVertex);
                    SetShape(frontLayer,sourceLabledVertex);
                    frontLayer.Add(sourceLabledVertex.Node);
                    sourceLabledVertex.Node.Location = new PointF((column + 1) * ColumnOffset, row * RowOffset);
                    DrawPairingEdge(sourceLabledVertex.Node, imageNode.Node, sourceNodePairing.NormalizedScore, frontLayer);
                    row++;
                }
                row++;
            }
        }

        private void SetShape(GoLayer frontLayer, GoLabeledVertexWrapper imageNode)
        {
            var shape = ((GoShape) imageNode.Node.Background);
            shape.BrushColor = colorCode.GetColor(imageNode.LabledVertex.Opcode);
            shape.PenWidth = 3;
            shape.PenColor = ImageGraphNodes.Contains(imageNode) ? Color.Green : Color.Orange;
            if (shape.BrushColor.GetBrightness() < 0.4)
            {
                imageNode.Node.Label.TextColor = Color.White;
            }
            shape.Size = new SizeF(400, 400);
            imageNode.Node.Text = imageNode.LabledVertex.Opcode.ToString() + " index:" + imageNode.LabledVertex.Index;
            if (imageNode.LabledVertex is CompoundedLabeledVertex)
            {
                imageNode.Node.Text += "multi";
            }
            imageNode.Node.Selected += Node_Selected;
            imageNode.Node.UnSelected += Node_UnSelected;
        }

        private void Node_UnSelected(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Node_Selected(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void DrawPairingEdge(GoTextNodeHoverable sourceGoNode, GoTextNodeHoverable imageGoNode, double score, GoLayer layer)
        {
            GoLink link = new GoLink();
            
            double pairingScore = score;
            if (pairingScore < 0)
            {
                pairingScore = 0;
            }
            Color edgeColor = Color.FromArgb(Convert.ToInt32(255 - 255 * pairingScore), Convert.ToInt32(255 * pairingScore), 0);
            link.ToolTipText = (pairingScore.ToString());
            link.Pen = new Pen(edgeColor);
            if (sourceGoNode == null || imageGoNode == null)
            {
                return;
            }
            link.ToPort = sourceGoNode.LeftPort;
            link.FromPort = imageGoNode.RightPort;
            layer.Add(link);
            link.PenWidth = 3;
        }
    }
}
