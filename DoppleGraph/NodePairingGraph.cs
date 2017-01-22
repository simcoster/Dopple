using GraphSimilarityByMatching;
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
    public partial class NodePairingGraph : Form2
    {
        private Dictionary<LabeledVertex, List<LabeledVertex>> _pairings;

        public NodePairingGraph(Dictionary<LabeledVertex, List<LabeledVertex>> pairings)
        {
            _pairings = pairings;
            InitializeComponent();
        }

        private void NodePairingGraph_Load(object sender, EventArgs e)
        {
            foreach (var smallGraph in _pairings.Keys)
            {

            }

        }
    }
}
