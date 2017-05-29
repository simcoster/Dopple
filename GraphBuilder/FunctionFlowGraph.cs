using Dopple.InstructionNodes;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple
{
    public class FunctionFlowGraph
    {
        public FunctionFlowGraph(MethodDefinition method, MethodDefinition typeInitializer)
        {
            Method = method;
            TypeInitializer = typeInitializer;
        }
        public List<InstructionNode> Nodes { get; set; } = new List<InstructionNode>();
        public MethodDefinition Method { get; set;}
        public MethodDefinition TypeCtor { get; set; }
        public MethodDefinition TypeInitializer { get; set; }
    }
}
