using DoppleTry2;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarityByMatching
{
    class MatchWeights
    {
        const int BigWeight = 100;
        static MatchWeights()
        {
            MatchWeight.Add(Code.Ret);
            CodeGroups.ArithmeticCodes.ForEach(x => MatchWeight.Add(x));
            CodeGroups.LdElemCodes.ForEach(x => MatchWeight.Add(x));
            CodeGroups.StElemCodes.ForEach(x => MatchWeight.Add(x));
            MatchWeight.AddRange(CodeGroups.CondJumpCodes);
        }
        public static List<Code> MatchWeight = new List<Code>();
    }
}
