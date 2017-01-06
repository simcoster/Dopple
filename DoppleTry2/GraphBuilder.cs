﻿using System;
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
            BackTrace();
            //RemoveHelperCodes();
            AddZeroNode();
            //MergeSimilarInstructions();
            //PostMergeBackTrace();
            SetInstructionIndexes();
            //Veirify();

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
                        MergeNodes(new[] { firstInst, secondInst });
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
            RemoveInstWrappers(InstructionNodes.Where(x => new[] { Code.Starg, Code.Starg_S }.Contains(x.Instruction.OpCode.Code)));
            RemoveInstWrappers(InstructionNodes.Where(x => CodeGroups.LdArgCodes.Contains(x.Instruction.OpCode.Code) && x.InliningProperties.Inlined));
            RemoveInstWrappers(InstructionNodes.Where(x => CodeGroups.CallCodes.Contains(x.Instruction.OpCode.Code) && x.InliningProperties.Inlined));
            RemoveInstWrappers(InstructionNodes.Where(x => x.Instruction.OpCode.Code == Code.Ret && x.InliningProperties.Inlined));
            RemoveInstWrappers(InstructionNodes.Where(x => x.Instruction.OpCode.Code == Code.Dup));
        }

        private void AddZeroNode()
        {
            var inst = Instruction.Create(typeof(OpCodes).GetFields().Select(x => x.GetValue(null)).Cast<OpCode>().First(x => x.Code == Code.Nop));
            var nodeZero = InstructionNodeFactory.GetInstructionWrapper(inst, metDef);

            foreach (var firstNode in InstructionNodes.Where(x => x.DataFlowBackRelated.Count == 0))
            {
                firstNode.DataFlowBackRelated.AddTwoWay(nodeZero);
            }
            InstructionNodes[0].ProgramFlowBackRoutes.AddTwoWay(nodeZero);
            InstructionNodes.Add(nodeZero);
            SetInstructionIndexes();
        }

        private void BackTrace()
        {
            foreach (var instWrapper in InstructionNodes.OrderByDescending(x => x.InstructionIndex))
            {
                var backTracers = _backTracers.Where(x => x.HandlesCodes.Contains(instWrapper.Instruction.OpCode.Code));
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
                MergeNodes(recursionGroup.ToArray());
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
                    MergeNodes(instsToMerge);
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
                        MergeNodes(toMerge.ToArray());
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
                    MergeNodes(ldArgSameBack);
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

        public void MergeNodes(IEnumerable<InstructionNode> nodesToMerge)
        {
            var nodeToKeep = nodesToMerge.ElementAt(0);
            foreach (var nodeToRemove in nodesToMerge.ToArray().Except(new[] { nodeToKeep }))
            {
                foreach (var backDataNode in nodeToRemove.DataFlowBackRelated.ToList())
                {
                    nodeToKeep.DataFlowBackRelated.AddTwoWay(backDataNode);
                    nodeToRemove.DataFlowBackRelated.RemoveTwoWay(backDataNode);
                }
                foreach (var forwardDataNode in nodeToRemove.DataFlowForwardRelated.ToList())
                {
                    var forwardBackRelated = forwardDataNode.DataFlowBackRelated.First(x => x.Argument == nodeToRemove);
                    forwardDataNode.DataFlowBackRelated.AddTwoWay(nodeToKeep, forwardBackRelated.ArgIndex);
                    forwardDataNode.DataFlowBackRelated.RemoveTwoWay(forwardBackRelated);
                }
                nodeToKeep.DataFlowBackRelated.CheckNumberings();
                foreach (var forwardFlowNode in nodeToRemove.ProgramFlowForwardRoutes.ToList())
                {
                    forwardFlowNode.ProgramFlowBackRoutes.RemoveTwoWay(nodeToRemove);
                    forwardFlowNode.ProgramFlowBackRoutes.AddTwoWay(nodeToKeep);
                }
                foreach (var backFlowNode in nodeToRemove.ProgramFlowBackRoutes.ToList())
                {
                    nodeToRemove.ProgramFlowBackRoutes.RemoveTwoWay(backFlowNode);
                    nodeToKeep.ProgramFlowBackRoutes.AddTwoWay(backFlowNode);
                }
                foreach(var forwardNode in nodeToRemove.ProgramFlowForwardAffecting.ToList())
                {
                    forwardNode.ProgramFlowBackAffected.AddTwoWay(nodeToKeep);
                    forwardNode.ProgramFlowBackAffected.RemoveTwoWay(nodeToRemove);
                }
                foreach (var backNode in nodeToRemove.ProgramFlowBackAffected.ToList())
                {
                    nodeToRemove.ProgramFlowBackAffected.RemoveTwoWay(backNode);
                    nodeToKeep.ProgramFlowBackAffected.AddTwoWay(backNode);
                }
                InstructionNodes.Remove(nodeToRemove);
                if (InstructionNodes.Any(x => x.DataFlowBackRelated.Any(y => y.Argument == nodeToRemove)
                                              || x.DataFlowForwardRelated.Any(y => y == nodeToRemove)
                                              || x.ProgramFlowBackRoutes.Any(y => y == nodeToRemove)
                                              || x.ProgramFlowForwardRoutes.Any(y => y == nodeToRemove)))
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