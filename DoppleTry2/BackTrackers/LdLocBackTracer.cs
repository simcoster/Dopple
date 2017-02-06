using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil.Cil;
using DoppleTry2.InstructionNodes;

namespace DoppleTry2.BackTrackers
{
    public class LdLocBackTracer : SingeIndexBackTracer
    {
        protected override IEnumerable<InstructionNode> GetDataflowBackRelatedArgGroup(InstructionNode instNode)
        {
            LocationLoadInstructionNode ldInstNode = (LocationLoadInstructionNode) instNode;
            Predicate<InstructionNode> nodeIsARelevantStloc = x => x is LocationStoreInstructionNode &&
                                    ((LocationStoreInstructionNode) x).LocIndex == ldInstNode.LocIndex;
            Predicate<InstructionNode> nodeIsARelevantStind = x => x is StIndInstructionNode &&
                                    ((StIndInstructionNode) x).AddressProvidingArgs.Any(y => new[] { Code.Ldloca, Code.Ldloca_S }.Contains(y.Instruction.OpCode.Code)
                                    && ((VariableDefinition)y.Instruction.Operand).Index == ldInstNode.LocIndex);

            return _SingleIndexBackSearcher.SearchBackwardsForDataflowInstrcutions(x => nodeIsARelevantStloc(x) ||  nodeIsARelevantStind(x) ,instNode);
        }

        public override Code[] HandlesCodes => CodeGroups.LdLocCodes;

        public LdLocBackTracer(List<InstructionNode> instructionNodes) : base(instructionNodes)
        {
        }
    }
}