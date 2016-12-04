using DoppleTry2.InstructionWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2
{
    public class IndexedArgument
    {
        public IndexedArgument(int argIndex, InstructionWrapper argument)
        {
            ArgIndex = argIndex;
            Argument = argument;
        }
        public int ArgIndex { get; set; }
        public InstructionWrapper Argument { get; set; }
    }
}
