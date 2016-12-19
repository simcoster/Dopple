using DoppleTry2.InstructionWrappers;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarity
{
    class GraphEditDistanceCalc
    {
        public const int NodeAdditionCost = 1;
        public const int NodeDeletionCost = 1;
        public const int NodeLabelSubstitutionCost = 1;

        public static int GetEditDistance(IEnumerable<InstructionWrapper> firstGraph, IEnumerable<InstructionWrapper> secondGraph)
        {
            List<EditOperation> OperationsToConcider
            Dictionary<int, List<EditOperation>> pathsAndCosts; 

            return 0;
        }

       
    }
}
