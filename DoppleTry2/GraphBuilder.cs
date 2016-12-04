using System;
using System.Collections.Generic;
using System.Linq;
using DoppleTry2.BackTrackers;
using DoppleTry2.InstructionModifiers;
using DoppleTry2.ProgramFlowHanlder;
using Mono.Cecil;
using Mono.Cecil.Cil;
using DoppleTry2.VerifierNs;
using DoppleTry2.InstructionWrappers;

namespace DoppleTry2
{
    public class GraphBuilder
    {
        private readonly IEnumerable<BackTracer> _backTracers;
        private readonly IEnumerable<ProgramFlowHandler> _flowHandlers;
        private readonly IEnumerable<IPostBackTraceModifier> _postBacktraceModifiers;
        private readonly IEnumerable<IPreBacktraceModifier> _preBacktraceModifiers;
        private readonly ProgramFlowManager _programFlowManager = new ProgramFlowManager();
        private MethodDefinition metDef;
        public List<InstructionWrapper> InstructionWrappers;

        public GraphBuilder(IEnumerable<InstructionWrapper> instWrappers)
        {
            InstructionWrappers = instWrappers.ToList();
        }
        public GraphBuilder(MethodDefinition methodDefinition)
        {
            metDef = methodDefinition;
            InstructionWrappers =
                methodDefinition.Body.Instructions.Select(x => InstructionWrapperFactory.GetInstructionWrapper(x, methodDefinition)).ToList();
            foreach (var inst in InstructionWrappers)
            {
                inst.InstructionIndex = InstructionWrappers.IndexOf(inst);
            }

            _preBacktraceModifiers = new IPreBacktraceModifier[] { new InlineCallModifier(), new RemoveUselessModifier() };

            //_postBacktraceModifiers = new IPostBackTraceModifier[] { new RecursionStArgModifier() };
            _postBacktraceModifiers = new IPostBackTraceModifier[] { };

            _backTracers =
                new BackTracer[]
                {
                    new StackPopBackTracer(InstructionWrappers),
                    new LdArgBacktracer(InstructionWrappers),
                    new LdStaticFieldBackTracer(InstructionWrappers),
                    //new LoadArrayElemBackTracer(InstructionWrappers),
                    new LoadFieldByStackBackTracer(InstructionWrappers),
                    new LoadMemoryByOperandBackTracer(InstructionWrappers),
                    new TypedReferenceBackTracer(InstructionWrappers)
                };
        }


        public List<InstructionWrapper> Run()
        {
            SetInstructionIndexes();
            _programFlowManager.AddFlowConnections(InstructionWrappers);
            PreInlineBackTrace();
            InlineFunctionCalls();
            SetInstructionIndexes();
            AddStArgHelpers();
            BackTrace();
            RemoveHelperCodes();
            MergeSimilarInstructions();
            PostMergeBackTrace();
            SetInstructionIndexes();
            Veirify();
            AddZeroNode();

            return InstructionWrappers;
        }

        private void PostMergeBackTrace()
        {
            LdElemBacktracer ldElemBacktracer = new LdElemBacktracer(InstructionWrappers);
            foreach(var instWrapper in InstructionWrappers)
            {
                if (!ldElemBacktracer.HandlesCodes.Contains(instWrapper.Instruction.OpCode.Code))
                {
                    continue;
                }
                ldElemBacktracer.AddBackDataflowConnections(instWrapper);
            }
        }

        private void MergeSimilarInstructions()
        {
            MergeLdArgs();
            MergeImmediateValue();
            //MergeLdLocs();
            MergeRecursionParalel();
            //MergeEquivilents();
        }

        private void MergeEquivilents()
        {
            List<InstructionWrapper> Merged = new List<InstructionWrapper>();
            foreach (var inst in InstructionWrappers.ToArray())
            {
                if (Merged.Contains(inst))
                {
                    continue;
                }
                var toMerge = InstructionWrappers
                                    .Where(x => x.Instruction.OpCode.Code == inst.Instruction.OpCode.Code)
                                    .Where(x =>  typeof(OpCodes).GetFields().Select(y => y.GetValue(null))
                                                .Cast<OpCode>().Where(y => y.StackBehaviourPop != StackBehaviour.Pop0).Select(y=> y.Code)
                                                .Contains(x.Instruction.OpCode.Code))
                                    .Where(x => x.BackDataFlowRelated.Select(y => y.Argument).SequenceEqual(inst.BackDataFlowRelated.Select(z => z.Argument)))
                                    .Where(x => !new[]{ Code.Ret}.Concat(CodeGroups.CallCodes).Contains(x.Instruction.OpCode.Code)) ;
                if (toMerge.Count() >1)
                {
                    MergeInsts(toMerge);
                    Merged.AddRange(toMerge);
                }
            }
        }

        private void RemoveHelperCodes()
        {
            var problematic = InstructionWrappers.FirstOrDefault(x => x.ForwardDataFlowRelated.GroupBy(y => y.Instruction).Any(y => y.Count() != y.First().BackDataFlowRelated.Count(z => z.Argument == x)));
            if (problematic != null)
            {

            }
            RemoveInstWrappers(InstructionWrappers.Where(x => CodeGroups.StLocCodes.Contains(x.Instruction.OpCode.Code)));
            RemoveInstWrappers(InstructionWrappers.Where(x => CodeGroups.LdLocCodes.Contains(x.Instruction.OpCode.Code)));
            RemoveInstWrappers(InstructionWrappers.Where(x => new[] { Code.Starg, Code.Starg_S }.Contains(x.Instruction.OpCode.Code)));
            RemoveInstWrappers(InstructionWrappers.Where(x => CodeGroups.LdArgCodes.Contains(x.Instruction.OpCode.Code) && x.InliningProperties.Inlined));
            RemoveInstWrappers(InstructionWrappers.Where(x => new[] { Code.Call, Code.Calli, Code.Callvirt }.Contains(x.Instruction.OpCode.Code) && x.InliningProperties.Inlined));
            RemoveInstWrappers(InstructionWrappers.Where(x => x.Instruction.OpCode.Code == Code.Ret && x.InliningProperties.Inlined));
            RemoveInstWrappers(InstructionWrappers.Where(x => x.Instruction.OpCode.Code == Code.Dup));
        }

        void AddStArgHelpers()
        {
            StArgAdder stArgAdder = new StArgAdder();
            stArgAdder.Modify(InstructionWrappers);
            SetInstructionIndexes();
        }

        private void AddZeroNode()
        {
            var inst = Instruction.Create(typeof(OpCodes).GetFields().Select(x => x.GetValue(null)).Cast<OpCode>().First(x => x.Code == Code.Nop));
            var nodeZero = InstructionWrapperFactory.GetInstructionWrapper(inst, metDef);

            foreach (var firstNode in InstructionWrappers.Where(x => x.BackDataFlowRelated.Count == 0 || x.FirstLineInstruction))
            {
                firstNode.BackDataFlowRelated.AddWithNewIndex(nodeZero);
            }
            foreach(var firstFromRecursion in InstructionWrappers.Where(x => BackSearcher.GetBackDataTree(x).Contains(x) && CodeGroups.LdArgCodes.Contains(x.Instruction.OpCode.Code)))
            {
                firstFromRecursion.BackDataFlowRelated.AddWithNewIndex(nodeZero);
            }
            InstructionWrappers.Add(nodeZero);
            SetInstructionIndexes();
        }

        private void BackTrace()
        {
            foreach (var instWrapper in InstructionWrappers.OrderByDescending(x => x.InstructionIndex))
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
            var recursionGroups = InstructionWrappers
                                    .Where(x => !CodeGroups.StArgCodes.Contains(x.Instruction.OpCode.Code))
                                    .GroupBy(x => new { x.Method.FullName, x.Instruction.Offset, x.Instruction.OpCode })
                                    .Where(x => x.Count() >1);
            foreach (var recursionGroup in recursionGroups)
            {
                MergeInsts(recursionGroup.ToArray());
            }
        }

        private void MergeImmediateValue()
        {
            foreach (var imeddiateValueNode in InstructionWrappers.Where(x => x is LdImmediateInstWrapper).Cast<LdImmediateInstWrapper>().ToList())
            {
                var instsToMerge = InstructionWrappers
                    .Where(x => x is LdImmediateInstWrapper)
                    .Cast<LdImmediateInstWrapper>()
                    .Where(x => imeddiateValueNode.ImmediateIntValue == x.ImmediateIntValue)
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
            foreach (var ldLocSameIndex in InstructionWrappers.Where(x => x is LocationLoadInstructionWrapper).Cast<LocationLoadInstructionWrapper>().GroupBy(x => x.LocIndex))
            {
                foreach(var ldLocWrapper in ldLocSameIndex)
                {
                    if (grouped.Contains(ldLocWrapper))
                    {
                        continue;
                    }
                    var toMerge = ldLocSameIndex
                                    .Where(x => x.BackDataFlowRelated.Select(y => y.Argument)
                                    .SequenceEqual(ldLocWrapper.BackDataFlowRelated.Select(z => z.Argument)))
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
            List<InstructionWrapper> doneWrappers = new List<InstructionWrapper>();
            var ldArgGroups = InstructionWrappers.Where(x => x is LdArgInstructionWrapper)
                                                 .Cast<FunctionArgInstWrapper>()
                                                 .GroupBy(x => new { x.ArgIndex, x.Method })
                                                 .Where(x => x.Count() >1)
                                                 .ToList();
            foreach(var ldArgGroup in ldArgGroups)
            {
                var ldArgGroupedByBackRelated = ldArgGroup
                                                    .Cast<InstructionWrapper>()
                                                    .GroupBySequence(x => x.BackDataFlowRelated.Select(y => y.Argument))
                                                    .Where(x => x.Count()>1)
                                                    .ToList();
                foreach (var ldArgSameBack in ldArgGroupedByBackRelated)
                {
                    MergeInsts(ldArgSameBack);
                }
            }
            SetInstructionIndexes();
        }

        private void PostBackTraceModifiers()
        {
            foreach (var modifier in _postBacktraceModifiers)
            {
                modifier.Modify(InstructionWrappers);
            }
        }


        private void InlineFunctionCalls()
        {
            foreach (var modifier in _preBacktraceModifiers)
            {
                modifier.Modify(InstructionWrappers);
            }
        }

        private void Veirify()
        {
            var verifiers = new Verifier[] {new StElemVerifier(InstructionWrappers), new StackPopPushVerfier(InstructionWrappers),
                                            new TwoWayVerifier(InstructionWrappers), new ArithmeticsVerifier(InstructionWrappers),
                                            new ArgIndexVerifier(InstructionWrappers), new LdElemVerifier(InstructionWrappers) };
            foreach (var instWrapper in InstructionWrappers.OrderByDescending(x => x.InstructionIndex))
            {
                foreach(var verifier in verifiers)
                {
                    verifier.Verify(instWrapper);
                }
            }
        }

        public void MergeInsts(IEnumerable<InstructionWrapper> Wrappers)
        {
            var instWrapperToKeep = Wrappers.ElementAt(0);
            foreach (var wrapperToRemove in Wrappers.ToArray().Except(new[] { instWrapperToKeep }))
            {
                foreach (var backDataNode in wrapperToRemove.BackDataFlowRelated.ToList())
                {
                    instWrapperToKeep.BackDataFlowRelated.AddWithExistingIndex(backDataNode);
                    wrapperToRemove.BackDataFlowRelated.RemoveTwoWay(backDataNode);
                }
                foreach (var forwardDataNode in wrapperToRemove.ForwardDataFlowRelated.ToList())
                {
                    var forwardBackRelated = forwardDataNode.BackDataFlowRelated.First(x => x.Argument == wrapperToRemove);
                    forwardDataNode.BackDataFlowRelated.AddWithExistingIndex(instWrapperToKeep, forwardBackRelated.ArgIndex);
                    forwardDataNode.BackDataFlowRelated.RemoveTwoWay(forwardBackRelated);
                }
                instWrapperToKeep.BackDataFlowRelated.CheckNumberings();
                foreach (var forwardFlowNode in wrapperToRemove.ForwardProgramFlow.ToList())
                {
                    forwardFlowNode.BackProgramFlow.RemoveTwoWay(wrapperToRemove);
                    forwardFlowNode.BackProgramFlow.AddTwoWay(instWrapperToKeep);
                }
                foreach (var backFlowNode in wrapperToRemove.BackProgramFlow.ToList())
                {
                    wrapperToRemove.BackProgramFlow.RemoveTwoWay(backFlowNode);
                    instWrapperToKeep.BackProgramFlow.AddTwoWay(backFlowNode);
                }
                InstructionWrappers.Remove(wrapperToRemove);
                if (InstructionWrappers.Any(x => x.BackDataFlowRelated.Any(y => y.Argument == wrapperToRemove)
                                              || x.ForwardDataFlowRelated.Any(y => y == wrapperToRemove)
                                              || x.BackProgramFlow.Any(y => y == wrapperToRemove)
                                              || x.ForwardProgramFlow.Any(y => y == wrapperToRemove)))
                {
                    throw new Exception("there's someone still pointing to the rmoeved");
                }
            }
        }

        public void RemoveInstWrappers(IEnumerable<InstructionWrapper> instsToRemove)
        {
            foreach (var wrapperToRemove in instsToRemove.ToArray())
            {
                foreach (var forInst in wrapperToRemove.ForwardDataFlowRelated.ToArray())
                {
                    var backArgToRemove = forInst.BackDataFlowRelated.First(x => x.Argument == wrapperToRemove);
                    forInst.BackDataFlowRelated.AddMultipleWithExistingIndex(wrapperToRemove.BackDataFlowRelated.Select(x => x.Argument), backArgToRemove.ArgIndex);
                    forInst.BackDataFlowRelated.RemoveTwoWay(backArgToRemove);
                    forInst.BackDataFlowRelated.CheckNumberings();
                }
                foreach(var backInst in wrapperToRemove.BackDataFlowRelated.ToArray())
                {
                    wrapperToRemove.BackDataFlowRelated.RemoveTwoWay(backInst);
                }
                foreach(var forwardBackFlowInst in wrapperToRemove.ForwardProgramFlow.ToList())
                {
                    forwardBackFlowInst.BackProgramFlow.RemoveAllTwoWay(x => x == wrapperToRemove);
                    forwardBackFlowInst.BackProgramFlow.AddRangeTwoWay(wrapperToRemove.BackProgramFlow);
                }
                foreach(var backFlowInst in wrapperToRemove.BackProgramFlow.ToList())
                {
                    wrapperToRemove.BackProgramFlow.RemoveTwoWay(backFlowInst);
                }
                InstructionWrappers.Remove(wrapperToRemove);
                if (InstructionWrappers.Any(x => x.BackDataFlowRelated.Any(y => y.Argument == wrapperToRemove)
                                               || x.ForwardDataFlowRelated.Any(y => y == wrapperToRemove)
                                               || x.BackProgramFlow.Any(y => y == wrapperToRemove)
                                               || x.ForwardProgramFlow.Any(y => y == wrapperToRemove)))
                {
                    throw new Exception("there's someone still pointing to the rmoeved");
                }
            }
            SetInstructionIndexes();
        }

        void PreInlineBackTrace()
        {
            LdLocBackTracer ldLocBackTracer = new LdLocBackTracer(InstructionWrappers);
            foreach(var instWrapper in InstructionWrappers.Where(x => ldLocBackTracer.HandlesCodes.Contains(x.Instruction.OpCode.Code)).OrderByDescending(x => x.InstructionIndex))
            {
                ldLocBackTracer.AddBackDataflowConnections(instWrapper);
            }
        }

        private void AddHelperRetInstructions()
        {
            AddHelperReturnInstsModifer addRetsModifier = new AddHelperReturnInstsModifer();
            addRetsModifier.Modify(InstructionWrappers);
        }

        public void SetInstructionIndexes()
        {
            foreach (var instWrapper in InstructionWrappers)
            {
                instWrapper.InstructionIndex = InstructionWrappers.IndexOf(instWrapper);
            }
        }
    }
}