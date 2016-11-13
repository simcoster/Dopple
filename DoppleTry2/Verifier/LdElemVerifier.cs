using DoppleTry2.InstructionWrappers;
using DoppleTry2.Verifier;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DoppleTry2.Verifier
{
    class LdElemVerifier : IVerifier
    {
        public void Verify(IEnumerable<InstructionWrapper> instructionWrappers)
        {
            foreach (var ldElem in instructionWrappers.Where(x => CodeGroups.LdElemCodes.Contains(x.Instruction.OpCode.Code)))
            {
            //    var arrayArg = ldElem.BackDataFlowRelated.ArgumentList.First(x => BackSearcher.GetStackPushAncestor(x.Argument)
            //                    .All(y => y.Instruction.OpCode.Code == Code.Newarr || (y is LdArgInstructionWrapper && ((LdArgInstructionWrapper)y).ArgType is ArrayType)));
            }
        }
    }
}
