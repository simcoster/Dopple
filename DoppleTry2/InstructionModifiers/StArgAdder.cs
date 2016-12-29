using DoppleTry2.BackTrackers;
using DoppleTry2.InstructionNodes;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.InstructionModifiers
{
    public class StArgAdder : IModifier
    {
        private static List<InstructionNode> InsertHelperSTargs(List<InstructionNode> instructionWrappers, InternalCallInstructionNode callInstWrapper)
        {
            var addedInstructions = new List<InstructionNode>();
            var calledFunc = callInstWrapper.CalledFunction;
            var stackPopBacktracer = new StackPopBackTracer(instructionWrappers);
            if (calledFunc.IsStatic)
            {
                for (int i = calledFunc.Parameters.Count - 1; i >= 0; i--)
                {
                    var argProvidingWrappers = stackPopBacktracer.SearchAndAddDataflowInstrcutions(callInstWrapper);
                    foreach (var argProvidingWrapper in argProvidingWrappers)
                    {
                        Instruction opcode = Instruction.Create(OpCodes.Starg, calledFunc.Parameters[i]);
                        var stArgWrapper = (StArgInstructionWrapper)InstructionNodeFactory.GetInstructionWrapper(opcode, calledFunc);
                        AddStArgInst(instructionWrappers, addedInstructions, calledFunc, i, argProvidingWrapper, stArgWrapper);
                    }
                }
            }
            else
            {
                for (int i = calledFunc.Parameters.Count; i >= 1; i--)
                {
                    var argProvidingWrappers = stackPopBacktracer.SearchAndAddDataflowInstrcutions(callInstWrapper);
                    foreach (var argProvidingWrapper in argProvidingWrappers)
                    {
                        Instruction opcode = Instruction.Create(OpCodes.Starg, calledFunc.Parameters[i -1]);
                        var stArgWrapper = (StArgInstructionWrapper)InstructionNodeFactory.GetInstructionWrapper(opcode, calledFunc);
                        AddStArgInst(instructionWrappers, addedInstructions, calledFunc, i, argProvidingWrapper, stArgWrapper);
                    }
                }
                var theThisArgProvidingWrappers = stackPopBacktracer.SearchAndAddDataflowInstrcutions(callInstWrapper);
                foreach (var argProvidingWrapper in theThisArgProvidingWrappers)
                {
                    Instruction opcode = Instruction.Create(OpCodes.Starg, calledFunc.Parameters[0]);
                    var stArgWrapper = new StThisArgInstructionWrapper(opcode, calledFunc);
                    AddStArgInst(instructionWrappers, addedInstructions, calledFunc, 0, argProvidingWrapper, stArgWrapper);
                }
            }
            return addedInstructions;
        }

        private static void AddStArgInst(List<InstructionNode> instructionWrappers, List<InstructionNode> addedInstructions, MethodDefinition calledFunc, int argIndex, InstructionNode argProvidingWrapper, StArgInstructionWrapper stArgWrapper)
        {
            stArgWrapper.Instruction.Offset = 99999;
            foreach (var forwardFlowInst in argProvidingWrapper.ProgramFlowForwardRoutes.ToList())
            {
                forwardFlowInst.ProgramFlowBackRoutes.RemoveTwoWay(argProvidingWrapper);
                forwardFlowInst.ProgramFlowBackRoutes.AddTwoWay(stArgWrapper);
            }
            stArgWrapper.ProgramFlowBackRoutes.AddTwoWay(argProvidingWrapper);
            stArgWrapper.DataFlowBackRelated.AddWithNewIndex(argProvidingWrapper);
            stArgWrapper.StackPopCount--;
            stArgWrapper.ArgIndex = argIndex;
            stArgWrapper.ProgramFlowResolveDone = true;
            argProvidingWrapper.StackPushCount--;
            instructionWrappers.Insert(instructionWrappers.IndexOf(argProvidingWrapper) + 1, stArgWrapper);
            addedInstructions.Add(stArgWrapper);
        }

        public void Modify(List<InstructionNode> instructionWrappers)
        {
            var callInstructions = instructionWrappers
                                    .Where(x => x is InternalCallInstructionNode && x.InliningProperties.Inlined)
                                    .OrderByDescending(x => instructionWrappers.IndexOf(x))
                                    .Cast<InternalCallInstructionNode>()
                                    .ToArray();
            foreach (var callInst in callInstructions)
            {
                InsertHelperSTargs(instructionWrappers, callInst);
            }
        }
    }
}
