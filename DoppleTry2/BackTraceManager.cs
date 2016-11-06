using System;
using System.Collections.Generic;
using System.Linq;
using DoppleTry2.BackTrackers;
using DoppleTry2.InstructionModifiers;
using DoppleTry2.ProgramFlowHanlder;
using Mono.Cecil;
using Mono.Cecil.Cil;
using DoppleTry2.Varifier;

namespace DoppleTry2
{
    public class BackTraceManager
    {
        private readonly IEnumerable<BackTracer> _backTracers;
        private readonly IEnumerable<ProgramFlowHandler> _flowHandlers;
        private readonly IEnumerable<IPostBackTraceModifier> _postBacktraceModifiers;
        private readonly IEnumerable<IPreBacktraceModifier> _preBacktraceModifiers;
        private readonly ProgramFlowManager _programFlowManager = new ProgramFlowManager();
        private MethodDefinition metDef;
        public List<InstructionWrapper> InstructionsWrappers;

        public BackTraceManager(IEnumerable<InstructionWrapper> instWrappers)
        {
            InstructionsWrappers = instWrappers.ToList();
        }
        public BackTraceManager(MethodDefinition methodDefinition)
        {
            metDef = methodDefinition;
            InstructionsWrappers =
                methodDefinition.Body.Instructions.Select(x => InstructionWrapperFactory.GetInstructionWrapper(x, methodDefinition)).ToList();
            foreach (var inst in InstructionsWrappers)
            {
                inst.InstructionIndex = InstructionsWrappers.IndexOf(inst);
            }

            _preBacktraceModifiers = new IPreBacktraceModifier[] { new InlineCallModifier(), new RemoveUselessModifier() };

            //_postBacktraceModifiers = new IPostBackTraceModifier[] { new RecursionStArgModifier() };
            _postBacktraceModifiers = new IPostBackTraceModifier[] { };

            _backTracers =
                new BackTracer[]
                {
                    new StackPopBackTracer(InstructionsWrappers),
                    new LdArgBacktracer(InstructionsWrappers),
                    new LdLocBackTracer(InstructionsWrappers),
                    new LdStaticFieldBackTracer(InstructionsWrappers),
                    new LoadArrayElemBackTracer(InstructionsWrappers),
                    new LoadFieldByStackBackTracer(InstructionsWrappers),
                    new LoadMemoryByOperandBackTracer(InstructionsWrappers),
                    new TypedReferenceBackTracer(InstructionsWrappers)
                };

            _flowHandlers =
               GetType()
                   .Assembly.GetTypes()
                   .Where(x => typeof(ProgramFlowHandler).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                   .Select(x => Activator.CreateInstance(x, InstructionsWrappers))
                   .Cast<ProgramFlowHandler>();
        }


        public List<InstructionWrapper> Run()
        {
            _programFlowManager.AddFlowConnections(InstructionsWrappers);
            PreInlineBackTrace();
            InlineFunctionCalls();
            AddStArgHelpers();
            BackTrace();

            MergeSimilarInstructions();
            //RemoveHelperCodes();

            AddZeroNode();
            SetInstructionIndexes();
            Veirify();

            return InstructionsWrappers;
        }

        private void MergeSimilarInstructions()
        {
            MergeLdArgs();
            MergeImmediateValue();
            MergeLdLocs();
            MergeRecursionParalel();
        }

        private void RemoveHelperCodes()
        {
            RemoveInstWrappers(InstructionsWrappers.Where(x => CodeGroups.LocStoreCodes.Contains(x.Instruction.OpCode.Code)));
            RemoveInstWrappers(InstructionsWrappers.Where(x => CodeGroups.LocLoadCodes.Contains(x.Instruction.OpCode.Code)));
            LdArgBacktracer ldArgBackTracer = new LdArgBacktracer(null);
            RemoveInstWrappers(InstructionsWrappers.Where(x => new[] { Code.Starg, Code.Starg_S }.Contains(x.Instruction.OpCode.Code) && x.Inlined));
            RemoveInstWrappers(InstructionsWrappers.Where(x => ldArgBackTracer.HandlesCodes.Contains(x.Instruction.OpCode.Code) && x.Inlined));
            RemoveInstWrappers(InstructionsWrappers.Where(x => new[] { Code.Call, Code.Calli, Code.Callvirt }.Contains(x.Instruction.OpCode.Code) && x.Inlined));
            RemoveInstWrappers(InstructionsWrappers.Where(x => x.Instruction.OpCode.Code == Code.Ret && x.Inlined));
            RemoveInstWrappers(InstructionsWrappers.Where(x => x.Instruction.OpCode.Code == Code.Dup));
        }

        void AddStArgHelpers()
        {
            StArgAdder stArgAdder = new StArgAdder();
            stArgAdder.Modify(InstructionsWrappers);
            SetInstructionIndexes();
        }

        private void AddZeroNode()
        {
            var inst = Instruction.Create(typeof(OpCodes).GetFields().Select(x => x.GetValue(null)).Cast<OpCode>().First(x => x.Code == Code.Nop));
            var nodeZero = InstructionWrapperFactory.GetInstructionWrapper(inst, metDef);

            foreach (var firstNode in InstructionsWrappers.Where(x => x.BackDataFlowRelated.ArgumentList.Count == 0 || x.FirstLineInstruction))
            {
                firstNode.AddBackDataflowTwoWaySingleIndex(new[] { nodeZero });
            }
            foreach(var firstFromRecursion in InstructionsWrappers.Where(x => BackSearcher.GetBackDataTree(x).Contains(x) && CodeGroups.LdArgCodes.Contains(x.Instruction.OpCode.Code)))
            {
                firstFromRecursion.AddBackDataflowTwoWaySingleIndex(new[] { nodeZero });
            }
            InstructionsWrappers.Add(nodeZero);
            SetInstructionIndexes();
        }

        private void BackTrace()
        {
            foreach (var instWrapper in InstructionsWrappers)
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
        }


        void MergeRecursionParalel()
        {
            var recursionGroups = InstructionsWrappers
                                    .Where(x => x.Instruction.OpCode.Code != Code.Starg && x.Instruction.OpCode.Code != Code.Starg_S)
                                    .GroupBy(x => new { x.Method.FullName, x.Instruction.Offset, x.Instruction.OpCode })
                                    .Where(x => x.Count() >1);
            foreach (var recursionGroup in recursionGroups)
            {
                MergeInsts(recursionGroup.ToArray());
            }
        }

        private void MergeImmediateValue()
        {
            foreach (var imeddiateValueNode in InstructionsWrappers.Where(x => x.ImmediateIntValue != null).ToList())
            {
                var instsToMerge = InstructionsWrappers
                    .Where(x => x.ImmediateIntValue != null)
                    .Where(x => imeddiateValueNode.ImmediateIntValue.Value == x.ImmediateIntValue.Value)
                    .Where(x => x.Method == imeddiateValueNode.Method)
                    .ToArray();
                if (instsToMerge.Length > 0)
                {
                    MergeInsts(instsToMerge);
                }
            }
            SetInstructionIndexes();
        }

        private void MergeLdLocs()
        {
            var grouped = new List<InstructionWrapper>();
            foreach (var ldLocSameIndex in InstructionsWrappers.Where(x => CodeGroups.LocLoadCodes.Contains(x.Instruction.OpCode.Code)).GroupBy(x => x.LocIndex))
            {
                foreach(var ldLocWrapper in ldLocSameIndex)
                {
                    if (grouped.Contains(ldLocWrapper))
                    {
                        continue;
                    }
                    var toMerge = ldLocSameIndex
                                    .Where(x => x.BackDataFlowRelated.ArgumentList.Select(y => y.Argument)
                                    .SequenceEqual(ldLocWrapper.BackDataFlowRelated.ArgumentList.Select(z => z.Argument)))
                                    .Except(grouped).ToArray();
                    if (toMerge.Count() > 1 )
                    {
                        MergeInsts(toMerge.ToArray());
                        grouped.AddRange(toMerge);
                    }
                }
            }
            SetInstructionIndexes();
        }

        private void MergeLdArgs()
        {
            var ldarg = new LdArgBacktracer(null);
            var argGroups = InstructionsWrappers.Where(x => x.ArgIndex != -1)
                                                 .Where(x => ldarg.HandlesCodes.Contains(x.Instruction.OpCode.Code))
                                                 .GroupBy(x => new { x.ArgIndex, x.Method });
            foreach (var argGroup in argGroups)
            {
                var instsToMerge = argGroup.Select(x => x).ToArray();
                if (instsToMerge.Length > 1)
                {
                    MergeInsts(instsToMerge);
                }
            }
            SetInstructionIndexes();
        }

        private void PostBackTraceModifiers()
        {
            foreach (var modifier in _postBacktraceModifiers)
            {
                modifier.Modify(InstructionsWrappers);
            }
        }


        private void InlineFunctionCalls()
        {
            foreach (var modifier in _preBacktraceModifiers)
            {
                modifier.Modify(InstructionsWrappers);
            }
        }

        private void Veirify()
        {
            var verifiers = new IVerifier[] { new StackPopPushVerfier(), new TwoWayVerifier() };
            foreach (var verifier in verifiers)
            {
                verifier.Verify(InstructionsWrappers);
            }
        }

        public void MergeInsts(InstructionWrapper[] Wrappers)
        {
            var instWrapperToKeep = Wrappers[0];
            foreach (var wrapperToRemove in Wrappers.Except(new[] { instWrapperToKeep }))
            {
                instWrapperToKeep.BackDataFlowRelated.AddSingleIndex(wrapperToRemove.BackDataFlowRelated);
                instWrapperToKeep.ForwardDataFlowRelated.AddSingleIndex(wrapperToRemove.ForwardDataFlowRelated);

                foreach (var backNode in wrapperToRemove.BackDataFlowRelated.ArgumentList)
                {
                    backNode.Argument.ForwardDataFlowRelated.ArgumentList.RemoveAll(x => x.Argument == wrapperToRemove);
                    backNode.Argument.ForwardDataFlowRelated.AddSingleIndex(instWrapperToKeep);
                }

                foreach (var forwardNode in wrapperToRemove.ForwardDataFlowRelated.ArgumentList)
                {
                    forwardNode.Argument.BackDataFlowRelated.ArgumentList.RemoveAll(x => x.Argument == wrapperToRemove);
                    forwardNode.Argument.BackDataFlowRelated.AddSingleIndex(instWrapperToKeep);
                }
                InstructionsWrappers.Remove(wrapperToRemove);
            }
        }

        public void RemoveInstWrappers(IEnumerable<InstructionWrapper> instsToRemove)
        {
            foreach (var instWrapper in instsToRemove.ToArray())
            {
                foreach (var forInst in instWrapper.ForwardDataFlowRelated.ArgumentList.Select(x => x.Argument).Distinct().ToArray())
                {
                    int backArgIndex = forInst.BackDataFlowRelated.ArgumentList.First(x => x.Argument == instWrapper).ArgIndex;
                    forInst.BackDataFlowRelated.AddSingleIndex(instWrapper.BackDataFlowRelated, backArgIndex);
                    //forInst.Argument.BackDataFlowRelated.ArgumentList.Remove(forInst.Argument.BackDataFlowRelated.ArgumentList.First(x => x.Argument == instWrapper));
                    //TODO should correct this, not preserving multiple connections to the same node
                    forInst.BackDataFlowRelated.ArgumentList.RemoveAll(x => x.Argument == instWrapper);
                }
                foreach (var backInst in instWrapper.BackDataFlowRelated.ArgumentList.ToArray())
                {
                    backInst.Argument.ForwardDataFlowRelated.AddSingleIndex(instWrapper.ForwardDataFlowRelated);
                    backInst.Argument.ForwardDataFlowRelated.ArgumentList.RemoveAll(x => x.Argument == instWrapper);
                }
                InstructionsWrappers.Remove(instWrapper);
            }
            SetInstructionIndexes();
        }

        void PreInlineBackTrace()
        {
            StackPopBackTracer stackPopBacktracer = new StackPopBackTracer(InstructionsWrappers);
            var singleStackPopWrappers = InstructionsWrappers
                                                .Where(x => stackPopBacktracer.HandlesCodes.Contains(x.Instruction.OpCode.Code))
                                                .Where(x => x.StackPopCount ==1)
                                                .OrderByDescending(x => InstructionsWrappers.IndexOf(x));
            foreach (var stackpopInst in singleStackPopWrappers)
            {
                stackPopBacktracer.AddBackDataflowConnectionsInFuncBoundry(stackpopInst);
            }
            //Maybe the problem is only about doing the singe pop first, let's try
        }

        private void AddHelperRetInstructions()
        {
            AddHelperRetsModifer addRetsModifier = new AddHelperRetsModifer();
            addRetsModifier.Modify(InstructionsWrappers);
        }

        public void SetInstructionIndexes()
        {
            foreach (var instWrapper in InstructionsWrappers)
            {
                instWrapper.InstructionIndex = InstructionsWrappers.IndexOf(instWrapper);
            }
        }
    }
}