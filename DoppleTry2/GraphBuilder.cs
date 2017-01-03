using System;
using System.Collections.Generic;
using System.Linq;
using DoppleTry2.BackTrackers;
using DoppleTry2.InstructionModifiers;
using DoppleTry2.ProgramFlowHanlder;
using Mono.Cecil;
using Mono.Cecil.Cil;
using DoppleTry2.VerifierNs;
using DoppleTry2.InstructionNodes;

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
        public List<InstructionNode> InstructionNodes;

        public GraphBuilder(IEnumerable<InstructionNode> instNodes)
        {
            InstructionNodes = instNodes.ToList();
        }
        public GraphBuilder(MethodDefinition methodDefinition)
        {
            metDef = methodDefinition;
            InstructionNodes =
                methodDefinition.Body.Instructions.Select(x => InstructionNodeFactory.GetInstructionWrapper(x, methodDefinition)).ToList();
            foreach (var inst in InstructionNodes)
            {
                inst.InstructionIndex = InstructionNodes.IndexOf(inst);
            }

            _preBacktraceModifiers = new IPreBacktraceModifier[] { new InlineCallModifier(), new RemoveUselessModifier() };

            _postBacktraceModifiers = new IPostBackTraceModifier[] { };

            _backTracers =
                new BackTracer[]
                {
                    new StackPopBackTracer(InstructionNodes),
                    new LdArgBacktracer(InstructionNodes),
                    new LdStaticFieldBackTracer(InstructionNodes),
                    new LoadFieldByStackBackTracer(InstructionNodes),
                    new LoadMemoryByOperandBackTracer(InstructionNodes),
                    new TypedReferenceBackTracer(InstructionNodes),
                    new ConditionionalsBackTracer(InstructionNodes)
                };
        }


        public List<InstructionNode> Run()
        {
            SetInstructionIndexes();
            _programFlowManager.AddFlowConnections(InstructionNodes);
            PreInlineBackTrace();
            InlineFunctionCalls();
            SetInstructionIndexes();
            //AddStArgHelpers();
            BackTrace();
            //RemoveHelperCodes();
            //MergeSimilarInstructions();
            PostMergeBackTrace();
            SetInstructionIndexes();
            Veirify();
            AddZeroNode();

            return InstructionNodes;
        }

        private void PostMergeBackTrace()
        {
            LdElemBacktracer ldElemBacktracer = new LdElemBacktracer(InstructionNodes);
            foreach(var instWrapper in InstructionNodes)
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
            //MergeRecursionParalel();
            //MergeEquivilentPairs();
        }

        private void MergeEquivilentPairs()
        {
            bool mergesWereDone;
            do
            {
                mergesWereDone = false;
                for (int i =0;  i < InstructionNodes.Count; i++)
                {
                    var firstInst = InstructionNodes[i];
                    var secondInst = InstructionNodes
                                        .Where(x => x != firstInst)
                                        .Where(x => x.Instruction.OpCode.Code == firstInst.Instruction.OpCode.Code)
                                        .Where(x => typeof(OpCodes).GetFields().Select(y => y.GetValue(null))
                                                    .Cast<OpCode>().Where(y => y.StackBehaviourPop != StackBehaviour.Pop0).Select(y => y.Code)
                                                    .Contains(x.Instruction.OpCode.Code))
                                        .Where(x => !new[] { Code.Ret }.Concat(CodeGroups.CallCodes).Contains(x.Instruction.OpCode.Code))
                                        .Where(x => x.DataFlowBackRelated.Equals(firstInst.DataFlowBackRelated))
                                        .Where(x => !x.DataFlowBackRelated.SelfFeeding)
                                        .FirstOrDefault();
                    if (secondInst != null)
                    {
                        MergeInsts(new[] { firstInst, secondInst });
                        mergesWereDone = true;
                    }
                }
            } while (mergesWereDone);
        }

        private bool DifferentArgumentsToSameInst(InstructionNode firstInst, InstructionNode secondInst)
        {
            var allForwardArgs = firstInst.DataFlowForwardRelated.Concat(secondInst.DataFlowForwardRelated).Distinct();
            foreach(var forwardArg in allForwardArgs)
            {
                var firstInstArg = forwardArg.DataFlowBackRelated.FirstOrDefault(x => x.Argument == firstInst);
                var secondInstArg = forwardArg.DataFlowBackRelated.FirstOrDefault(x => x.Argument == secondInst);
                if (firstInstArg == null || secondInstArg == null)
                {
                    continue;
                }
                if (firstInstArg.ArgIndex != secondInstArg.ArgIndex)
                {
                    return true;
                }
            }
            return false;
        }

        private void RemoveHelperCodes()
        {
            RemoveInstWrappers(InstructionNodes.Where(x => CodeGroups.StLocCodes.Contains(x.Instruction.OpCode.Code)));
            RemoveInstWrappers(InstructionNodes.Where(x => CodeGroups.LdLocCodes.Contains(x.Instruction.OpCode.Code)));
            //RemoveInstWrappers(InstructionNodes.Where(x => new[] { Code.Starg, Code.Starg_S }.Contains(x.Instruction.OpCode.Code)));
            RemoveInstWrappers(InstructionNodes.Where(x => CodeGroups.LdArgCodes.Contains(x.Instruction.OpCode.Code) && x.InliningProperties.Inlined));
            //TODO remove
            //RemoveInstWrappers(InstructionNodes.Where(x => new[] { Code.Call, Code.Calli, Code.Callvirt }.Contains(x.Instruction.OpCode.Code) && x.InliningProperties.Inlined));
            RemoveInstWrappers(InstructionNodes.Where(x => x.Instruction.OpCode.Code == Code.Ret && x.InliningProperties.Inlined));
            RemoveInstWrappers(InstructionNodes.Where(x => x.Instruction.OpCode.Code == Code.Dup));
        }

        void AddStArgHelpers()
        {
            StArgAddModifier stArgAdder = new StArgAddModifier();
            stArgAdder.Modify(InstructionNodes);
            SetInstructionIndexes();
        }

        private void AddZeroNode()
        {
            var inst = Instruction.Create(typeof(OpCodes).GetFields().Select(x => x.GetValue(null)).Cast<OpCode>().First(x => x.Code == Code.Nop));
            var nodeZero = InstructionNodeFactory.GetInstructionWrapper(inst, metDef);

            foreach (var firstNode in InstructionNodes.Where(x => x.DataFlowBackRelated.Count == 0))
            {
                firstNode.DataFlowBackRelated.AddTwoWay(nodeZero);
            }
            //foreach(var firstFromRecursion in InstructionNodes.Where(x => BackSearcher.GetBackDataTree(x).Contains(x) && CodeGroups.LdArgCodes.Contains(x.Instruction.OpCode.Code)))
            //{
            //    firstFromRecursion.DataFlowBackRelated.AddWithNewIndex(nodeZero);
            //}
            InstructionNodes[0].ProgramFlowBackRoutes.AddTwoWay(nodeZero);
            InstructionNodes.Add(nodeZero);
            SetInstructionIndexes();
        }

        private void BackTrace()
        {
            foreach (var instWrapper in InstructionNodes.OrderByDescending(x => x.InstructionIndex))
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
            var recursionGroups = InstructionNodes
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
            foreach (var imeddiateValueNode in InstructionNodes.Where(x => x is LdImmediateInstNode).Cast<LdImmediateInstNode>().ToList())
            {
                var instsToMerge = InstructionNodes
                    .Where(x => x is LdImmediateInstNode)
                    .Cast<LdImmediateInstNode>()
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
            var grouped = new List<InstructionNode>();
            foreach (var ldLocSameIndex in InstructionNodes.Where(x => x is LocationLoadInstructionNode).Cast<LocationLoadInstructionNode>().GroupBy(x => x.LocIndex))
            {
                foreach(var ldLocWrapper in ldLocSameIndex)
                {
                    if (grouped.Contains(ldLocWrapper))
                    {
                        continue;
                    }
                    var toMerge = ldLocSameIndex
                                    .Where(x => x.DataFlowBackRelated.Select(y => y.Argument)
                                    .SequenceEqual(ldLocWrapper.DataFlowBackRelated.Select(z => z.Argument)))
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
            var doneWrappers = new List<InstructionNode>();
            var ldArgGroups = InstructionNodes.Where(x => x is LdArgInstructionNode)
                                                 .Cast<FunctionArgInstNode>()
                                                 .GroupBy(x => new { x.ArgIndex, x.Method })
                                                 .Where(x => x.Count() >1)
                                                 .ToList();
            foreach(var ldArgGroup in ldArgGroups)
            {
                var ldArgGroupedByBackRelated = ldArgGroup
                                                    .Cast<InstructionNode>()
                                                    .GroupBySequence(x => x.DataFlowBackRelated.Select(y => y.Argument))
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
                modifier.Modify(InstructionNodes);
            }
        }


        private void InlineFunctionCalls()
        {
            foreach (var modifier in _preBacktraceModifiers)
            {
                modifier.Modify(InstructionNodes);
            }
        }

        private void Veirify()
        {
            var verifiers = new Verifier[] {new StElemVerifier(InstructionNodes), new StackPopPushVerfier(InstructionNodes),
                                            new TwoWayVerifier(InstructionNodes), new ArithmeticsVerifier(InstructionNodes),
                                            new ArgIndexVerifier(InstructionNodes), new LdElemVerifier(InstructionNodes) };
            foreach (var instWrapper in InstructionNodes.OrderByDescending(x => x.InstructionIndex))
            {
                foreach(var verifier in verifiers)
                {
                    verifier.Verify(instWrapper);
                }
            }
        }

        public void MergeInsts(IEnumerable<InstructionNode> Wrappers)
        {
            var instWrapperToKeep = Wrappers.ElementAt(0);
            foreach (var wrapperToRemove in Wrappers.ToArray().Except(new[] { instWrapperToKeep }))
            {
                foreach (var backDataNode in wrapperToRemove.DataFlowBackRelated.ToList())
                {
                    instWrapperToKeep.DataFlowBackRelated.AddTwoWay(backDataNode);
                    wrapperToRemove.DataFlowBackRelated.RemoveTwoWay(backDataNode);
                }
                foreach (var forwardDataNode in wrapperToRemove.DataFlowForwardRelated.ToList())
                {
                    var forwardBackRelated = forwardDataNode.DataFlowBackRelated.First(x => x.Argument == wrapperToRemove);
                    forwardDataNode.DataFlowBackRelated.AddTwoWay(instWrapperToKeep, forwardBackRelated.ArgIndex);
                    forwardDataNode.DataFlowBackRelated.RemoveTwoWay(forwardBackRelated);
                }
                instWrapperToKeep.DataFlowBackRelated.CheckNumberings();
                foreach (var forwardFlowNode in wrapperToRemove.ProgramFlowForwardRoutes.ToList())
                {
                    forwardFlowNode.ProgramFlowBackRoutes.RemoveTwoWay(wrapperToRemove);
                    forwardFlowNode.ProgramFlowBackRoutes.AddTwoWay(instWrapperToKeep);
                }
                foreach (var backFlowNode in wrapperToRemove.ProgramFlowBackRoutes.ToList())
                {
                    wrapperToRemove.ProgramFlowBackRoutes.RemoveTwoWay(backFlowNode);
                    instWrapperToKeep.ProgramFlowBackRoutes.AddTwoWay(backFlowNode);
                }
                InstructionNodes.Remove(wrapperToRemove);
                if (InstructionNodes.Any(x => x.DataFlowBackRelated.Any(y => y.Argument == wrapperToRemove)
                                              || x.DataFlowForwardRelated.Any(y => y == wrapperToRemove)
                                              || x.ProgramFlowBackRoutes.Any(y => y == wrapperToRemove)
                                              || x.ProgramFlowForwardRoutes.Any(y => y == wrapperToRemove)))
                {
                    throw new Exception("there's someone still pointing to the rmoeved");
                }
            }
            SetInstructionIndexes();
        }

        public void RemoveInstWrappers(IEnumerable<InstructionNode> instsToRemove)
        {
            foreach (var wrapperToRemove in instsToRemove.ToArray())
            {
                foreach (var forInst in wrapperToRemove.DataFlowForwardRelated.ToArray())
                {
                    var backArgToRemove = forInst.DataFlowBackRelated.First(x => x.Argument == wrapperToRemove);
                    forInst.DataFlowBackRelated.AddTwoWay(wrapperToRemove.DataFlowBackRelated.Select(x => x.Argument), backArgToRemove.ArgIndex);
                    forInst.DataFlowBackRelated.RemoveTwoWay(backArgToRemove);
                    forInst.DataFlowBackRelated.CheckNumberings();
                }
                foreach(var backInst in wrapperToRemove.DataFlowBackRelated.ToArray())
                {
                    wrapperToRemove.DataFlowBackRelated.RemoveTwoWay(backInst);
                }
                foreach(var forwardBackFlowInst in wrapperToRemove.ProgramFlowForwardRoutes.ToList())
                {
                    forwardBackFlowInst.ProgramFlowBackRoutes.RemoveAllTwoWay(x => x == wrapperToRemove);
                    forwardBackFlowInst.ProgramFlowBackRoutes.AddTwoWay(wrapperToRemove.ProgramFlowBackRoutes);
                }
                foreach(var backFlowInst in wrapperToRemove.ProgramFlowBackRoutes.ToList())
                {
                    wrapperToRemove.ProgramFlowBackRoutes.RemoveTwoWay(backFlowInst);
                }
                InstructionNodes.Remove(wrapperToRemove);
                if (InstructionNodes.Any(x => x.DataFlowBackRelated.Any(y => y.Argument == wrapperToRemove)
                                               || x.DataFlowForwardRelated.Any(y => y == wrapperToRemove)
                                               || x.ProgramFlowBackRoutes.Any(y => y == wrapperToRemove)
                                               || x.ProgramFlowForwardRoutes.Any(y => y == wrapperToRemove)))
                {
                    throw new Exception("there's someone still pointing to the removed");
                }
            }
            SetInstructionIndexes();
        }

        void PreInlineBackTrace()
        {
            LdLocBackTracer ldLocBackTracer = new LdLocBackTracer(InstructionNodes);
            foreach(var instWrapper in InstructionNodes.Where(x => ldLocBackTracer.HandlesCodes.Contains(x.Instruction.OpCode.Code)).OrderByDescending(x => x.InstructionIndex))
            {
                ldLocBackTracer.AddBackDataflowConnections(instWrapper);
            }
        }

        private void AddHelperRetInstructions()
        {
            AddHelperReturnInstsModifer addRetsModifier = new AddHelperReturnInstsModifer();
            addRetsModifier.Modify(InstructionNodes);
        }

        public void SetInstructionIndexes()
        {
            foreach (var instWrapper in InstructionNodes)
            {
                instWrapper.InstructionIndex = InstructionNodes.IndexOf(instWrapper);
            }
        }
    }
}