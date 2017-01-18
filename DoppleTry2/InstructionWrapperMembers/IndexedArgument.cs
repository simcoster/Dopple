using DoppleTry2.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2
{
    public class IndexedArgument
    {
        public IndexedArgument(int argIndex, InstructionNode argument, ArgList containingList)
        {
            ArgIndex = argIndex;
            Argument = argument;
            ContainingList = containingList;
        }
        public int ArgIndex { get; set; }
        public InstructionNode Argument { get; set; }
        public ArgList ContainingList { get; set; }
        public IndexedArgument MirrorArg { get; set; }
    }
}
