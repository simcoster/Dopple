using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoppleWebDemo.Controllers.Helpers
{
    public class NodesAndEdges
    {
        public List<NodeForJS> Nodes { get; set; } = new List<NodeForJS>();
        public List<EdgeForJS> Edges { get; set; } = new List<EdgeForJS>();
    }
}