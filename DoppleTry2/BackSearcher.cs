using DoppleTry2.InstructionNodes;
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
        public static IEnumerable<InstructionNode> GetStackPushAncestor(InstructionNode startInst, List<InstructionNode> visited = null)
        {
            if (visited == null)
            {
                visited = new List<InstructionNode>();
            }
            InstructionNode instWrapper = startInst;
            while (true)
            {
                if (visited.Contains(instWrapper))
                {
                    return new InstructionNode[] {};
                }
                visited.Add(instWrapper);
                switch (instWrapper.DataFlowBackRelated.Count)
                {
                    case 0:
                        return new[] { instWrapper };
                    case 1:
                        instWrapper = instWrapper.DataFlowBackRelated[0].Argument;
                        break;
                    default:
                        return instWrapper.DataFlowBackRelated.SelectMany(x => GetStackPushAncestor(x.Argument, visited));
                }
            }
        }

        public static IEnumerable<InstructionNode> GetBackDataTree(InstructionNode startInst, List<InstructionNode> visited = null)
        {
            if (visited == null)
            {
                visited = new List<InstructionNode>();
            }
            if (visited.Contains(startInst))
            {
                return new InstructionNode[0];
            }
            visited.Add(startInst);
            visited.AddRange(startInst.DataFlowBackRelated.SelectMany(x => GetBackDataTree(x.Argument, visited)).ToArray());
            return visited.Distinct();
        }

        public static IEnumerable<InstructionNode> GetBackFlowTree(InstructionNode startInst, List<InstructionNode> visited = null)
        {
            if (visited == null)
            {
                visited = new List<InstructionNode>();
            }
            if (visited.Contains(startInst))
            {
                return new InstructionNode[0];
            }
            visited.Add(startInst);
            visited.AddRange(startInst.ProgramFlowBackRoutes.SelectMany(x => GetBackDataTree(x, visited)).ToArray());
            return visited.Distinct();
        }

        public static IEnumerable<InstructionNode> GetForwardFlowTree(InstructionNode startInst, List<InstructionNode> visited = null)
        {
            if (visited == null)
            {
                visited = new List<InstructionNode>();
            }
            if (visited.Contains(startInst))
            {
                return new InstructionNode[0];
            }
            visited.Add(startInst);
            visited.AddRange(startInst.ProgramFlowForwardRoutes.SelectMany(x => GetForwardFlowTree(x, visited)).ToArray());
            return visited.Distinct();
        }

        public static bool HaveCommonStackPushAncestor(InstructionNode firstInstruction, InstructionNode secondInstructions)
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

        public static bool SameOrEquivilent(InstructionNode firstAncestor, InstructionNode secondAncestor)
        {
            return firstAncestor == secondAncestor ||
                   (firstAncestor.Instruction.OpCode.Code == secondAncestor.Instruction.OpCode.Code &&
                    firstAncestor.Instruction.Operand == secondAncestor.Instruction.Operand);
        }
    }
}
