using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dopple.InstructionNodes;
using Mono.Cecil;

namespace DoppleWebDemo.Controllers.Helpers
{
    public class NodesAndEdges
    {
        public List<NodeForJS> NodesJS { get; set; } = new List<NodeForJS>();
        public List<EdgeForJS> DataEdgesJS { get; set; } = new List<EdgeForJS>();
        public List<EdgeForJS> FlowEdgesJS { get; set; } = new List<EdgeForJS>();
        public List<InstructionNode> Nodes { get; internal set; }
        public MethodDefinition Method { get; set; }
    }
}