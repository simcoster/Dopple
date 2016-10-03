using System;
using System.Collections.Generic;
using System.Linq;
using DoppleTry2.BackTrackers;
using DoppleTry2.InstructionModifiers;
using DoppleTry2.ProgramFlowHanlder;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DoppleTry2
{
    public class BackTraceManager
    {
        private List<InstructionWrapper> _instructionsWrappers;
        private readonly IEnumerable<BackTracer> _backTracers;
        private readonly IEnumerable<IModifier> _modifiers;
        private readonly IEnumerable<ProgramFlowHandler> _flowHandlers;
        private MethodDefinition metDef;

        public BackTraceManager(MethodDefinition methodDefinition)
        {
            metDef = methodDefinition;
            _instructionsWrappers =
                methodDefinition.Body.Instructions.Select(x => new InstructionWrapper(x, methodDefinition)).ToList();
            foreach (var inst in _instructionsWrappers)
            {
                inst.InstructionIndex = _instructionsWrappers.IndexOf(inst);
            }

            _modifiers =
                GetType()
                    .Assembly.GetTypes()
                    .Where(x => typeof(IModifier).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                    .Select(Activator.CreateInstance)
                    .Cast<IModifier>();
            _backTracers =
                new BackTracer[]
                {
                    new ImmediateValueBackTracer(_instructionsWrappers),
                    new StackPopBackTracer(_instructionsWrappers),
                    new LdArgBacktracer(_instructionsWrappers),
                    new LdLocBackTracer(_instructionsWrappers),
                    new LdStaticFieldBackTracer(_instructionsWrappers),
                    new LoadArrayElemBackTracer(_instructionsWrappers),
                    new LoadFieldByStackBackTracer(_instructionsWrappers),
                    new LoadMemoryByOperandBackTracer(_instructionsWrappers),
                    new TypedReferenceBackTracer(_instructionsWrappers)
                };

            _flowHandlers =
               GetType()
                   .Assembly.GetTypes()
                   .Where(x => typeof(ProgramFlowHandler).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                   .Select(x => Activator.CreateInstance(x, _instructionsWrappers))
                   .Cast<ProgramFlowHandler>();
        }

        public List<InstructionWrapper> Run()
        {
            foreach (var modifier in _modifiers)
            {
                modifier.Modify(_instructionsWrappers);
            }

            foreach (var instWrapper in _instructionsWrappers)
            {
                var flowHandlers = _flowHandlers.Where(x => x.HandledCodes.Contains(instWrapper.Instruction.OpCode.Code));
                foreach (var flowHandler in flowHandlers)
                {
                    flowHandler.SetForwardExecutionFlowInsts(instWrapper);
                }
            }

            foreach (var instWrapper in _instructionsWrappers)
            {
                var backTracers = _backTracers.Where(x => x.HandlesCodes.Contains(instWrapper.Instruction.OpCode.Code));
                if (backTracers.Count() == 0)
                {
                    Console.WriteLine("Element with no backtracer!!!! " + instWrapper.Instruction.ToString());
                }
                foreach (var backTracer in backTracers)
                {
                    backTracer.AddBackDataflowConnections(instWrapper);
                }
            }

            return _instructionsWrappers;

            var ldloc = new LdLocBackTracer(null);
            RemoveInstWrappers(_instructionsWrappers.Where(x => ldloc.HandlesCodes.Contains(x.Instruction.OpCode.Code)));
            RemoveInstWrappers(_instructionsWrappers.Where(x => ldloc._storingCodes.Contains(x.Instruction.OpCode.Code)));

            var ldarg = new LdArgBacktracer(null);
            foreach(int argIndex in _instructionsWrappers.Select(x => x.ArgIndex).Distinct().ToList())
            {
                var instsToMerge = _instructionsWrappers
                    .Where(x => ldarg.HandlesCodes.Contains(x.Instruction.OpCode.Code))
                    .Where(x => x.ArgIndex == argIndex).ToArray();
                if (instsToMerge.Length > 0)
                {
                    MergeInsts(instsToMerge);
                }
            }
            var ldImeddiate = new LdArgBacktracer(null);
            foreach (int argIndex in _instructionsWrappers.Select(x => x).Distinct().ToList())
            {
                var instsToMerge = _instructionsWrappers
                    .Where(x => ldarg.HandlesCodes.Contains(x.Instruction.OpCode.Code))
                    .Where(x => x.ArgIndex == argIndex).ToArray();
                if (instsToMerge.Length > 0)
                {
                    MergeInsts(instsToMerge);
                }
            }
            RemoveInstWrappers(_instructionsWrappers.Where(x => x.Inlined == true));
            return _instructionsWrappers;
        }

        public void MergeInsts(InstructionWrapper[] Wrappers)
        {
            var instWrapperToKeep = Wrappers[0];
            foreach (var wrapper in Wrappers.Except(new[] { instWrapperToKeep}))
            {
                instWrapperToKeep.BackDataFlowRelated.AddRange(wrapper.BackDataFlowRelated);
                instWrapperToKeep.ForwardDataFlowRelated.AddRange(wrapper.ForwardDataFlowRelated);
                foreach(var removeMeFrom in instWrapperToKeep.BackDataFlowRelated.Concat(instWrapperToKeep.ForwardDataFlowRelated))
                {
                    removeMeFrom.ForwardDataFlowRelated.Remove(wrapper);
                    removeMeFrom.BackDataFlowRelated.Remove(wrapper);
                }
                _instructionsWrappers.Remove(wrapper);
            }
        }

        public void RemoveInstWrappers(IEnumerable<InstructionWrapper> instsToRemove)
        {
            var instWrappersToRemove = new List<InstructionWrapper>();
            foreach (var instWrapper in instsToRemove)
            {
                foreach (var forInst in instWrapper.ForwardDataFlowRelated)
                {
                    forInst.BackDataFlowRelated.AddRange(instWrapper.BackDataFlowRelated);
                    forInst.BackDataFlowRelated.Remove(instWrapper);
                    instWrappersToRemove.Add(instWrapper);
                }
                foreach (var backInst in instWrapper.BackDataFlowRelated)
                {
                    backInst.ForwardDataFlowRelated.AddRange(instWrapper.ForwardDataFlowRelated);
                    backInst.ForwardDataFlowRelated.Remove(instWrapper);
                    instWrappersToRemove.Add(instWrapper);
                }
            }
            _instructionsWrappers = _instructionsWrappers.Except(instWrappersToRemove).ToList();
        }
    }
}