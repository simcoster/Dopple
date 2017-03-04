using Dopple.InstructionNodes;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.InstructionModifiers
{
    public class MethodAndNode
    {
        List<InstructionNode> Nodes { get; set; }
        MethodDefinition Method { get; set; }
    }
}
