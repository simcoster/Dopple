using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using DoppleTry2.BackTrackers;
using DoppleTry2.ProgramFlowHanlder;
using DoppleTry2.InstructionWrappers;

namespace DoppleTry2.InstructionModifiers
{
    class InlineCallModifier : IPreBacktraceModifier
    {
        public static readonly Code[] CallOpCodes = CodeGroups.CallCodes;
        ProgramFlowManager programFlowHanlder = new ProgramFlowManager();

        public void Modify(List<InstructionWrapper> instructionWrappers)
        {
            int recursionInstanceIndex = 1;
            foreach (var nestedCallInstWrapper in instructionWrappers.Where(x => x is CallInstructionWrapper).Cast<CallInstructionWrapper>().ToArray())
            {
                var inlinedInstWrappers = GetDeepInlineRec(nestedCallInstWrapper, new List<MethodDefinition>() { instructionWrappers[0].Method });
                if (inlinedInstWrappers.Count == 0)
                {
                    continue;
                }
                int instWrapperIndex = instructionWrappers.IndexOf(nestedCallInstWrapper);
                instructionWrappers.InsertRange(instWrapperIndex + 1, inlinedInstWrappers);
                foreach (var inlinedInstWrapper in inlinedInstWrappers)
                {
                    inlinedInstWrapper.InliningProperties.Inlined = true;
                }
                if (nestedCallInstWrapper.CalledFunction.FullName == nestedCallInstWrapper.Method.FullName)
                {
                    foreach(var inst in inlinedInstWrappers)
                    {
                        inst.InliningProperties.RecursionInstanceIndex = recursionInstanceIndex;
                        inst.InliningProperties.Recursive = true;
                    }
                    recursionInstanceIndex++;
                }
            }

            foreach (var nestedCallInstWrapper in instructionWrappers.Where(x => x is CallInstructionWrapper && x.InliningProperties.Inlined))
            {
                int nestedCallIndex = instructionWrappers.IndexOf(nestedCallInstWrapper);
                instructionWrappers[nestedCallIndex + 1].BackProgramFlow.AddTwoWay(nestedCallInstWrapper);
                nestedCallInstWrapper.ProgramFlowResolveDone = true;
            }

            foreach (var retCall in instructionWrappers.Where(x => x.Instruction.OpCode.Code == Code.Ret && x != instructionWrappers.Last()))
            {
                int retCallIndex = instructionWrappers.IndexOf(retCall);
                instructionWrappers[retCallIndex + 1].BackProgramFlow.AddTwoWay(retCall);
                retCall.ProgramFlowResolveDone = true;
                if (retCall.Instruction.OpCode.Code == Code.Ret)
                {
                    if (retCall.Method.ReturnType.FullName == "System.Void")
                    {
                        retCall.StackPushCount = 0;
                    }
                    else
                    {
                        retCall.StackPushCount = 1;
                    }
                }
            }
        }

        List<InstructionWrapper> GetDeepInlineRec(InstructionWrapper callInstWrapper, List<MethodDefinition> callSequence)
        {
            if (!(callInstWrapper is CallInstructionWrapper))
            {
                throw new Exception("only user functions should be inlined");
            }
            var calledFunc = (MethodDefinition)callInstWrapper.Instruction.Operand;
            if (callSequence.Count(x => x == calledFunc) > 1)
            {
                return new List<InstructionWrapper>();
            }
            else
            {
                var callSequenceClone = new List<MethodDefinition>(callSequence);
                callSequenceClone.Add(calledFunc);
                var calledFuncInstructions = calledFunc.Body.Instructions.ToList();
                var calledFuncInstWrappers = calledFuncInstructions.Select(x => InstructionWrapperFactory.GetInstructionWrapper(x, calledFunc)).ToList();
                programFlowHanlder.AddFlowConnections(calledFuncInstWrappers);
                InFuncBackTrace(calledFuncInstWrappers);
                foreach (var nestedCallInstWrapper in calledFuncInstWrappers.Where(x => x is CallInstructionWrapper).ToArray())
                {
                    int instWrapperIndex = calledFuncInstWrappers.IndexOf(nestedCallInstWrapper);
                    var inlinedInstWrappers = GetDeepInlineRec(nestedCallInstWrapper, callSequenceClone);
                    if (inlinedInstWrappers.Count > 0)
                    {
                        calledFuncInstWrappers.InsertRange(instWrapperIndex + 1, inlinedInstWrappers);
                        nestedCallInstWrapper.ForwardProgramFlow.Clear();
                        inlinedInstWrappers[0].BackProgramFlow.AddTwoWay(nestedCallInstWrapper);
                    }
                }
                callInstWrapper.InliningProperties.Inlined = true;
                return calledFuncInstWrappers;
            }
        }

        private void InFuncBackTrace(List<InstructionWrapper> calledFuncInstructions)
        {
            LdLocBackTracer LoadLocationBacktracer = new LdLocBackTracer(calledFuncInstructions);
            foreach(var inst in calledFuncInstructions.Where(x => LoadLocationBacktracer.HandlesCodes.Contains(x.Instruction.OpCode.Code)).OrderByDescending(x => x.InstructionIndex))
            {
                LoadLocationBacktracer.AddBackDataflowConnections(inst);
            }
        }
    }
}
