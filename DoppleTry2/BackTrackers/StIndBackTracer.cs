using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoppleTry2.InstructionNodes;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    class StIndAddressBackTracer : BackTracer
    {
        public StIndAddressBackTracer(List<InstructionNode> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        public override Code[] HandlesCodes
        {
            get
            {
                return CodeGroups.StIndCodes;
            }
        }
        protected override void InnerAddBackDataflowConnections(InstructionNode currentInst)
        {
            StIndInstructionNode stIndInst = (StIndInstructionNode) currentInst;
            var addressNodes = currentInst.DataFlowBackRelated.Where(x => x.ArgIndex == 0)
                                .SelectMany(x => TraceBackwardsLdStLocs(x.Argument));
            if (addressNodes.Any(x => !IsProvidingAddress(x)))
            {
                throw new Exception("some args don't provide address");
            }
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


        private bool IsProvidingAddress(InstructionNode addressBackArg)
        {
            var addressProvidingCodes = new[] {Code.Ldarga_S, Code.Ldarga, Code.Ldelema, Code.Ldflda, Code.Ldloca, Code.Ldloca_S,
                          Code.Ldsflda, Code.Localloc,Code.Refanyval };
            if ( addressProvidingCodes.Contains(addressBackArg.Instruction.OpCode.Code))
            {
                return true;
            }
            if (addressBackArg is LdArgInstructionNode && ((LdArgInstructionNode)addressBackArg).ParamDefinition.IsOut)
            {
                return true;
            }
            return false;
        }
    }
}
