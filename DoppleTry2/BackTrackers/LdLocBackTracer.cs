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
                                    ((LocationStoreInstructionNode) x).LocIndex == ldInstNode.LocIndex &&
                                    x.Method == instNode.Method;
            Predicate<InstructionNode> nodeIsARelevantStind = x => x is StIndInstructionNode &&
                                    ((StIndInstructionNode) x).AddressProvidingArgs.Any(y => new[] { Code.Ldloca, Code.Ldloca_S }.Contains(y.Instruction.OpCode.Code)
                                    && ((VariableDefinition)y.Instruction.Operand).Index == ldInstNode.LocIndex
                                    && y.Method == instNode.Method);

            return _SingleIndexBackSearcher.SearchBackwardsForDataflowInstrcutions(x => nodeIsARelevantStloc(x) ||  nodeIsARelevantStind(x) ,instNode);
        }

        public override Code[] HandlesCodes => CodeGroups.LdLocCodes;

        public LdLocBackTracer(List<InstructionNode> instructionNodes) : base(instructionNodes)
        {
        }
    }
}