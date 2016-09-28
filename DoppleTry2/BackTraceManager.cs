using System;
using System.Collections.Generic;
using System.Linq;
using DoppleTry2.BackTrackers;
using DoppleTry2.InstructionModifiers;
using DoppleTry2.ProgramFlowHanlder;
using Mono.Cecil;

namespace DoppleTry2
{
    public class BackTraceManager
    {
        private readonly List<InstructionWrapper> _instructionsWrappers;
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
        }
    }
}