using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2
{
    public static class BackSearcher
    {
        public static List<InstructionWrapper> SearchBackwardsForDataflowInstrcutions(List<InstructionWrapper> InstructionWrappers, Func<InstructionWrapper, bool> predicate,
           InstructionWrapper startInstruction)
        {
            List<InstructionWrapper> indexes = SafeSearchBackwardsForDataflowInstrcutions(InstructionWrappers, predicate, startInstruction);
            if (indexes.Count == 0)
            {
                //throw new Exception("Reached first instWrapper without correct one found");
            }
            return indexes;
        }

        public static List<InstructionWrapper> SafeSearchBackwardsForDataflowInstrcutions(List<InstructionWrapper> InstructionWrappers, Func<InstructionWrapper, bool> predicate,
           InstructionWrapper startInstruction)
        {
            return startInstruction.BackProgramFlow.SelectMany(x => SafeSearchBackwardsForDataflowInstrcutionsRec(InstructionWrappers,predicate, x, new List<InstructionWrapper>())).ToList();
        }

        public static List<InstructionWrapper> SafeSearchBackwardsForDataflowInstrcutionsRec(List<InstructionWrapper> InstructionWrappers, Func<InstructionWrapper, bool> predicate,
        InstructionWrapper startInstruction, List<InstructionWrapper> visitedInstructions)
        {
            if (visitedInstructions == null)
            {
                visitedInstructions = new List<InstructionWrapper>();
            }
            var foundInstructions = new List<InstructionWrapper>();
            int index = InstructionWrappers.IndexOf(startInstruction);
            if (index < 0)
                return new List<InstructionWrapper>();
            while (true)
            {
                var currInstruction = InstructionWrappers[index];
                if (visitedInstructions.Contains(currInstruction))
                {
                    break;
                }
                else
                {
                    visitedInstructions.Add(currInstruction);
                }

                if (predicate.Invoke(currInstruction))
                {
                    foundInstructions.Add(currInstruction);
                    break;
                }
                else if (currInstruction.BackProgramFlow.Count == 0)
                {
                    break;
                }
                else if (currInstruction.BackProgramFlow.Count == 1)
                {
                    index = InstructionWrappers.IndexOf(currInstruction.BackProgramFlow[0]);
                }
                else
                {
                    foreach (var instructionWrapper in currInstruction.BackProgramFlow)
                    {
                        IEnumerable<InstructionWrapper> branchindexes =
                            SafeSearchBackwardsForDataflowInstrcutionsRec(InstructionWrappers, predicate, instructionWrapper, visitedInstructions);
                        foundInstructions.AddRange(branchindexes);
                    }
                    break;
                }
            }
            return foundInstructions;
        }

        public static IEnumerable<InstructionWrapper> GetStackPushAncestor(InstructionWrapper startInst, List<InstructionWrapper> visited = null)
        {
            if (visited == null)
            {
                visited = new List<InstructionWrapper>();
            }
            var instWrapper = startInst;
            while (true)
            {
                visited.Add(instWrapper);
                if (visited.Contains(instWrapper))
                {
                    return new InstructionWrapper[0];
                }
                switch (instWrapper.BackDataFlowRelated.ArgumentList.Count)
                {
                    case 0:
                        return new[] { instWrapper };
                    case 1:
                        instWrapper = instWrapper.BackDataFlowRelated.ArgumentList[0].Argument;
                        break;
                    default:
                        return instWrapper.BackDataFlowRelated.ArgumentList.SelectMany(x => GetStackPushAncestor(x.Argument, visited));
                }
            }
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
