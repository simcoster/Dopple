using Dopple;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarityByMatching
{
    class CodeInfo
    {
        readonly HashSet<Code> CriticalCodes = new HashSet<Code>(new Code[0]
                                                .Concat(CodeGroups.LdElemCodes)
                                                .Concat(CodeGroups.StArgCodes)
                                                .Concat(CodeGroups.StElemCodes)
                                                .Concat(CodeGroups.StLocCodes));
        readonly HashSet<Code> ImportantCodes = new HashSet<Code>(new Code[] { Code.Sub, Code.Sub_Ovf, Code.Sub_Ovf_Un, Code.Div, Code.Div_Un }
                                                                                .Concat(CodeGroups.CallCodes)
                                                                                .Concat(CodeGroups.CondJumpCodes)
                                                                                .Concat(CodeGroups.LdLocCodes)
                                                                                .Concat(CodeGroups.LdArgCodes)
                                                                                .ToArray());
        readonly HashSet<Code> UnimportantCodes;
        readonly Dictionary<HashSet<Code>, IndexImportance> CodeIndexImportance;
        public CodeInfo ()
        {
            UnimportantCodes = new HashSet<Code>(CodeGroups.AllOpcodes.Select(x => x.Code).Except(CriticalCodes).Except(ImportantCodes).ToArray());
            CodeIndexImportance = new Dictionary<HashSet<Code>, IndexImportance>() { { CriticalCodes, IndexImportance.Critical }, { ImportantCodes, IndexImportance.Important }, { UnimportantCodes, IndexImportance.NotImportant } };
        }

        public IndexImportance GetIndexImportance(Code code)
        {
            return CodeIndexImportance[CodeIndexImportance.Keys.First(x => x.Contains(code))];
        }
    }
    enum IndexImportance
    {
        Critical,
        Important,
        NotImportant
    }
}
