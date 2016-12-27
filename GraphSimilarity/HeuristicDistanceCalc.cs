using DoppleTry2.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarity
{
    class HeuristicDistanceCalc
    {
        public static int HeuristicNodeDistance(IEnumerable<InstructionNode> firstGraph, IEnumerable<InstructionNode> secondGraph)
        {
            return firstGraph.Select(x => x.Instruction).Except(secondGraph.Select(x => x.Instruction)).Count();
        }
    }
}
