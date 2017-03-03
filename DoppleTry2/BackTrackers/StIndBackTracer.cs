using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;
using Mono.Cecil.Cil;

namespace Dopple.BackTracers
{
    class StIndAddressBackTracer : BackTracer
    {
        public override Code[] HandlesCodes
        {
            get
            {
                return CodeGroups.StIndCodes;
            }
        }
        protected override void BackTraceDataFlowSingle(InstructionNode currentInst)
        {
            StIndInstructionNode stIndInst = (StIndInstructionNode) currentInst;
            var addressNodes = currentInst.DataFlowBackRelated.Where(x => x.ArgIndex == 0)
                                .SelectMany(x => TraceBackwardsLdStLocs(x.Argument));
            if (addressNodes.Any(x => !GetAddressType(x).HasValue))
            {
                throw new Exception("some args don't provide address");
            }
            stIndInst.AddressType = GetAddressType(addressNodes.First()).Value;
            stIndInst.AddressProvidingArgs = addressNodes.ToList();
        }

        private IEnumerable<InstructionNode> TraceBackwardsLdStLocs(InstructionNode startInstruction, List<InstructionNode> visited = null)
        {
            if (visited ==null)
            {
                visited = new List<InstructionNode>();
            }
            if (startInstruction is LocationLoadInstructionNode || startInstruction is LocationStoreInstructionNode || (startInstruction is LdArgInstructionNode && startInstruction.DataFlowBackRelated.Any()))
            {
                return startInstruction.DataFlowBackRelated.SelectMany(x => TraceBackwardsLdStLocs(x.Argument, visited));
            }
            else
            {
                return new []{ startInstruction};
            }
        }


        private AddressType? GetAddressType(InstructionNode addressBackArg)
        {
            if ( new[] { Code.Ldarga_S, Code.Ldarga}.Contains(addressBackArg.Instruction.OpCode.Code))
            {
                return AddressType.Argument;
            }
            if (new[] { Code.Ldelema }.Contains(addressBackArg.Instruction.OpCode.Code))
            {
                return AddressType.ArrayElem;
            }
            if (new[] { Code.Ldflda, Code.Ldsflda }.Contains(addressBackArg.Instruction.OpCode.Code))
            {
                return AddressType.Field;
            }
            if (new[] { Code.Ldloca, Code.Ldloca_S }.Contains(addressBackArg.Instruction.OpCode.Code))
            {
                return AddressType.LocalVar;
            }
            if (new[] { Code.Localloc, Code.Refanyval }.Contains(addressBackArg.Instruction.OpCode.Code))
            {
                return AddressType.GeneralData;
            }
            if (CodeGroups.LdArgCodes.Contains(addressBackArg.Instruction.OpCode.Code) && addressBackArg.DataFlowBackRelated.Count ==0)
            {
                return AddressType.GeneralData;
            }
            return null;
        }
    }
}
