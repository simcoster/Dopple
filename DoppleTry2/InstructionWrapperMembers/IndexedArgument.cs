using Dopple.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple
{
    public class IndexedArgument
    {
        public IndexedArgument(int argIndex, InstructionNode argument, CoupledIndexedArgList containingList)
        {
            ArgIndex = argIndex;
            Argument = argument;
            ContainingList = containingList;
        }
        public int ArgIndex { get; set; }
        public InstructionNode Argument { get; set; }
        public CoupledIndexedArgList ContainingList { get; set; }
        public IndexedArgument MirrorArg { get; set; }
    }
}
