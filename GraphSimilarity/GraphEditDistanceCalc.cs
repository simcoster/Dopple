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
            http://www.springer.com/cda/content/document/cda_downloaddocument/9783319272511-c2.pdf?SGWID=0-0-45-1545097-p177820399
                 //step 1 = take all the first graph nodes
                 //step 2 = concider for each one, replacing with each one of the second graph
                 //step 3 = concider for each one, deleting
                 //continue until no more nodes left of the first graph
            IEnumerable<InstructionWrapper> secondGraphNodesToResolve = new List<InstructionWrapper>(secondGraph);
            List<EditPath> PathsToConcider = new List<EditPath>();
            foreach(var firstGraphNode in firstGraph)
            {
                foreach(var secondGraphNode in secondGraph)
                {
                    NodeLabelSubstitutionCost
                    PathsToConcider.Add()
                }
            }
            IEnumerable<InstructionWrapper> firstGraphNodesToResolve = new List<InstructionWrapper>(firstGraph);

            return 0;
        }

       
    }
}
