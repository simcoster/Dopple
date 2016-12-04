using DoppleTry2.InstructionWrappers;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2
{
    public static class BackSearcher
    {
        public static IEnumerable<InstructionWrapper> GetStackPushAncestor(InstructionWrapper startInst, List<InstructionWrapper> visited = null)
        {
            var nonModifyingCodes = new Code[0].Concat(CodeGroups.LdLocCodes).Concat(CodeGroups.StLocCodes);
            if (visited == null)
            {
                visited = new List<InstructionWrapper>();
            }
            var instWrapper = startInst;
            while (true)
            {
                if (visited.Contains(instWrapper))
                {
                    return new InstructionWrapper[] {};
                }
                visited.Add(instWrapper);
                switch (instWrapper.BackDataFlowRelated.Count)
                {
                    case 0:
                        return new[] { instWrapper };
                    case 1:
                        instWrapper = instWrapper.BackDataFlowRelated[0].Argument;
                        break;
                    default:
                        return instWrapper.BackDataFlowRelated.SelectMany(x => GetStackPushAncestor(x.Argument, visited));
                }
            }
        }

        public static IEnumerable<InstructionWrapper> GetBackDataTree(InstructionWrapper startInst, List<InstructionWrapper> visited = null)
        {
            if (visited == null)
            {
                visited = new List<InstructionWrapper>();
            }
            if (visited.Contains(startInst))
            {
                return new InstructionWrapper[0];
            }
            visited.Add(startInst);
            visited.AddRange(startInst.BackDataFlowRelated.SelectMany(x => GetBackDataTree(x.Argument, visited)).ToArray());
            return visited.Distinct();
        }

        public static IEnumerable<InstructionWrapper> GetBackFlowTree(InstructionWrapper startInst, List<InstructionWrapper> visited = null)
        {
            if (visited == null)
            {
                visited = new List<InstructionWrapper>();
            }
            if (visited.Contains(startInst))
            {
                return new InstructionWrapper[0];
            }
            visited.Add(startInst);
            visited.AddRange(startInst.BackProgramFlow.SelectMany(x => GetBackDataTree(x, visited)).ToArray());
            return visited.Distinct();
        }

        public static bool HaveCommonStackPushAncestor(InstructionWrapper firstInstruction, InstructionWrapper secondInstructions)
        {
            var firstAncestors = GetStackPushAncestor(firstInstruction).ToArray();
            var secondAncestors = GetStackPushAncestor(secondInstructions).ToArray();

            foreach (var firstAncestor in firstAncestors)
            {
                foreach (var secondAncestor in secondAncestors)
                {
                    if (SameOrEquivilent(firstAncestor, secondAncestor))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool SameOrEquivilent(InstructionWrapper firstAncestor, InstructionWrapper secondAncestor)
        {
            return firstAncestor == secondAncestor ||
                   (firstAncestor.Instruction.OpCode.Code == secondAncestor.Instruction.OpCode.Code &&
                    firstAncestor.Instruction.Operand == secondAncestor.Instruction.Operand);
        }
    }
}
