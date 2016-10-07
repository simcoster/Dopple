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

            var ldarg = new LdArgBacktracer(null);
            foreach(int argIndex in _instructionsWrappers.Select(x => x.ArgIndex).Distinct().ToList())
            {
                var instsToMerge = _instructionsWrappers
                    .Where(x => ldarg.HandlesCodes.Contains(x.Instruction.OpCode.Code))
                    .Where(x => x.ArgIndex == argIndex).ToArray();
                if (instsToMerge.Length > 0)
                {
                   // MergeInsts(instsToMerge);
                }
            }

            foreach (var imeddiateValueNode in _instructionsWrappers.Where(x => x.ImmediateIntValue != null).ToList())
            {
                var instsToMerge = _instructionsWrappers
                    .Where(x => x.ImmediateIntValue != null && x.ImmediateIntValue.Value == x.ImmediateIntValue.Value).ToArray();
                if (instsToMerge.Length > 0)
                {
                   // MergeInsts(instsToMerge);
                }
            }

            var ldloc = new LdLocBackTracer(null);

            foreach (int locIndex in _instructionsWrappers
                .Where(x => x.LocIndex != -1)
                .Select(x => x.LocIndex).Distinct().ToList())
            {
                var sameLocInsts = _instructionsWrappers.Where(x => x.LocIndex == locIndex &&
                                        ldloc.HandlesCodes.Contains(x.Instruction.OpCode.Code)).ToList();
                var mergedInsts = new List<InstructionWrapper>();
                foreach (var locInst in sameLocInsts)
                {
                    if (mergedInsts.Contains(locInst))
                    {
                        continue;
                    }
                    var instsToMerge = sameLocInsts.Where(x => x.BackDataFlowRelated == locInst.BackDataFlowRelated).ToList();
                    if (instsToMerge.Count() > 1)
                    {
                     //   MergeInsts(instsToMerge.ToArray());
                    }
                    mergedInsts.AddRange(instsToMerge);
                }              
            }

           // RemoveInstWrappers(_instructionsWrappers.Where(x => ldloc._storingCodes.Contains(x.Instruction.OpCode.Code)));
          //  RemoveInstWrappers(_instructionsWrappers.Where(x => ldloc.HandlesCodes.Contains(x.Instruction.OpCode.Code)));
          //  RemoveInstWrappers(_instructionsWrappers.Where(x => x.Inlined == true));

            var inst = Instruction.Create(typeof(OpCodes).GetFields().Select(x => x.GetValue(null)).Cast<OpCode>().First(x => x.Code == Code.Nop));
            var nodeZero = new InstructionWrapper(inst, metDef);

            foreach (var firstNode in _instructionsWrappers.Where(x => x.BackDataFlowRelated.ArgumentList.Count == 0))
            {
                firstNode.AddBackTwoWaySingleIndex(new[] { nodeZero });
            }
            _instructionsWrappers.Add(nodeZero);

            foreach(var node in _instructionsWrappers)
            {
                foreach(var backNode in node.BackDataFlowRelated.ArgumentList)
                {
                    if (!backNode.Argument.ForwardDataFlowRelated.ArgumentList.Select(x => x.Argument).Contains(node))
                    {
                        throw new Exception();
                    }
                }
                foreach (var backNode in node.ForwardDataFlowRelated.ArgumentList)
                {
                    if (!backNode.Argument.BackDataFlowRelated.ArgumentList.Select(x => x.Argument).Contains(node))
                    {
                        throw new Exception();
                    }
                }
            }

            return _instructionsWrappers;
        }

        public void MergeInsts(InstructionWrapper[] Wrappers)
        {
            var instWrapperToKeep = Wrappers[0];
            foreach (var wrapperToRemove in Wrappers.Except(new[] { instWrapperToKeep}))
            {
                instWrapperToKeep.BackDataFlowRelated.AddSingleIndex(wrapperToRemove.BackDataFlowRelated);
                instWrapperToKeep.ForwardDataFlowRelated.AddSingleIndex(wrapperToRemove.ForwardDataFlowRelated);

                foreach( var backNode in wrapperToRemove.BackDataFlowRelated.ArgumentList)
                {
                    backNode.Argument.ForwardDataFlowRelated.ArgumentList.RemoveAll(x => x.Argument == wrapperToRemove);
                    backNode.Argument.ForwardDataFlowRelated.AddSingleIndex(instWrapperToKeep);
                }

                foreach (var forwardNode in wrapperToRemove.ForwardDataFlowRelated.ArgumentList)
                {
                    forwardNode.Argument.BackDataFlowRelated.ArgumentList.RemoveAll(x => x.Argument == wrapperToRemove);
                    forwardNode.Argument.BackDataFlowRelated.AddSingleIndex(instWrapperToKeep);
                }
                _instructionsWrappers.Remove(wrapperToRemove);
            }
        }

        public void RemoveInstWrappers(IEnumerable<InstructionWrapper> instsToRemove)
        {
            var instWrappersToRemove = new List<InstructionWrapper>();
            foreach (var instWrapper in instsToRemove)
            {
                foreach (var forInst in instWrapper.ForwardDataFlowRelated.ArgumentList)
                {
                    forInst.Argument.BackDataFlowRelated.AddSingleIndex(instWrapper.BackDataFlowRelated);
                    forInst.Argument.BackDataFlowRelated.ArgumentList.RemoveAll(x => x.Argument == instWrapper);
                }
                foreach (var backInst in instWrapper.BackDataFlowRelated.ArgumentList)
                {
                    backInst.Argument.ForwardDataFlowRelated.AddSingleIndex(instWrapper.ForwardDataFlowRelated);
                    backInst.Argument.ForwardDataFlowRelated.ArgumentList.RemoveAll(x => x.Argument == instWrapper);
                }
                instWrappersToRemove.Add(instWrapper);
            }
            _instructionsWrappers = _instructionsWrappers.Except(instWrappersToRemove).ToList();
        }
    }
}