using DoppleTry2.InstructionWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarity
{
    class HeuristicDistanceCalc
    {
        public static int HeuristicNodeDistance(IEnumerable<InstructionWrapper> firstGraph, IEnumerable<InstructionWrapper> secondGraph)
        {
            return firstGraph.Select(x => x.Instruction).Except(secondGraph.Select(x => x.Instruction)).Count();
        }
    }
}
