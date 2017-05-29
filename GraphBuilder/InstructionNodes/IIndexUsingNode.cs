using System.Collections.Generic;

namespace Dopple.InstructionNodes
{
    internal interface IIndexUsingNode
    {
        IEnumerable<InstructionNode> IndexNodes { get; }
    }
}