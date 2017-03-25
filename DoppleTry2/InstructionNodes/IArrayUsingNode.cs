using System.Collections;
using System.Collections.Generic;

namespace Dopple.InstructionNodes
{
    internal interface IArrayUsingNode
    {
        IEnumerable<InstructionNode> ArrayBackArgs { get; }
    }
}