using System;
using System.Collections.Generic;
using System.Linq;
using Dopple.BackTracers;
using Dopple.InstructionModifiers;
using Dopple.ProgramFlowHanlder;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Dopple.VerifierNs;
using Dopple.InstructionNodes;

namespace Dopple
{
    public class GraphBuilder
    {
        private readonly IEnumerable<ProgramFlowHandler> _flowHandlers;
        private readonly IEnumerable<IPostBackTraceModifier> _postBacktraceModifiers;
        private readonly IEnumerable<IPreBacktraceModifier> _preBacktraceModifiers;
        private readonly ProgramFlowManager _programFlowManager = new ProgramFlowManager();
        private MethodDefinition metDef;
        public List<InstructionNode> InstructionNodes;
        private readonly BackTraceManager _backTraceManager = new BackTraceManager();
        private InstructionNodeFactory _InstructionNodeFactory = new InstructionNodeFactory();
        private VirtualMethodResolver _VirtualMethodResolver = new VirtualMethodResolver();
        private CallInliner _inlineCallModifier;
        //ConditionionalsTracer _ConditionalBacktracer = new ConditionionalsTracer();
        Verifier[] verifiers;

        public GraphBuilder(IEnumerable<InstructionNode> instNodes)
        {
            InstructionNodes = instNodes.ToList();
            InitInliner();
        }
        public GraphBuilder(MethodDefinition methodDefinition)
        {
            metDef = methodDefinition;
            InstructionNodes =
                methodDefinition.Body.Instructions.SelectMany(x => _InstructionNodeFactory.GetInstructionNodes(x, methodDefinition)).ToList();
            foreach (var inst in InstructionNodes)
            {
                inst.InstructionIndex = InstructionNodes.IndexOf(inst);
            }
            InitInliner();
        }
        private void InitInliner()
        {
            verifiers = new Verifier[] {new StElemVerifier(InstructionNodes), new StackPopPushVerfier(InstructionNodes),
                                            new TwoWayVerifier(InstructionNodes), new ArithmeticsVerifier(InstructionNodes),
                                            new ArgIndexVerifier(InstructionNodes), new LdElemVerifier(InstructionNodes),
                                            new ArgRemovedVerifier(InstructionNodes)};
            _inlineCallModifier = new CallInliner(_InstructionNodeFactory);
        }


        public List<InstructionNode> Run()
        {
            bool shouldRerun = true;
            bool isFirstRun = true;
            SetInstructionIndexes();
            while (shouldRerun)
            {
                _programFlowManager.AddFlowConnections(InstructionNodes);
                if (isFirstRun)
                {
                    try
                    {

                        _backTraceManager.DataTraceInFunctionBounds(InstructionNodes);
                    }
                    catch (StackPopException stackPopException)
                    {
                        return stackPopException.problematicRoute;
                    }
                }
                InlineFunctionCalls();
                SetInstructionIndexes();
                BackTraceOutsideFuncBoundry();
                //MergeSingleOperationNodes();
                //MergeSimilarInstructions();
                //MergeEquivilentPairs();
                //PostMergeBackTrace();
                //RecursionFix();
                ResolveVirtualMethods(out shouldRerun);
                //SetInstructionIndexes();
                isFirstRun = false;

            }
            //RemoveHelperCodes();
            AddZeroNode();
            //Verify();

            return InstructionNodes;
        }

        private void BackTraceOutsideFuncBoundry()
        {
            _backTraceManager.BackTraceOutsideFunctionBounds(InstructionNodes);
        }

        private void ResolveVirtualMethods(out bool inliningWasDone)
        {
            _VirtualMethodResolver.ResolveVirtualMethods(InstructionNodes, out inliningWasDone);
        }

        private void RecursionFix()
        {
            //TODO remove
            _inlineCallModifier.Verifiers = verifiers;
            _inlineCallModifier.MergeRecursiveNodes(InstructionNodes);            
        }

        private void MergeSingleOperationNodes()
        {
            //TODO deal with this inside backtrace manager
            //new SingleConditionOperationUnitBackTracer().AddBackDataflowConnections(InstructionNodes);
            //new SingleArithmeticWithConstantBacktracer().AddBackDataflowConnections(InstructionNodes);

            //var blah = InstructionNodes.Where(x => x.SingleUnitBackRelated.Count > 0);
            //var frontMostNodes = InstructionNodes.Where(x => x.SingleUnitBackRelated.Count > 0 && x.SingleUnitForwardRelated.Count == 0);
            //foreach (var frontMostNode in frontMostNodes.ToArray())
            //{
            //    Queue<InstructionNode> singleUnitBackNodes = new Queue<InstructionNode>();
            //    frontMostNode.SingleUnitBackRelated.ForEach(x => singleUnitBackNodes.Enqueue(x));
            //    while(singleUnitBackNodes.Count >0)
            //    {
            //        var currNode = singleUnitBackNodes.Dequeue();
            //        currNode.SingleUnitBackRelated.ForEach(x => singleUnitBackNodes.Enqueue(x));
            //        currNode.MergeInto(frontMostNode,false);
            //        InstructionNodes.Remove(currNode);
            //        frontMostNode.SingleUnitNodes.Add(currNode);
            //    }
            //}
        }

        private void MarkNodesAsDataEquivilent()
        {

        }

        private void MergeSimilarInstructions()
        {
            MergeLdArgs();
            MergeImmediateValue();
            MergeLoadNulls();
        }

        private void MergeLoadNulls()
        {
            MergeNodes(InstructionNodes.Where(x => x.Instruction.OpCode.Code == Code.Ldnull));
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
                    var secondInstOptions = InstructionNodes
                                        .Where(x => x != firstInst)
                                        .Where(x => x.Instruction.OpCode.Code == firstInst.Instruction.OpCode.Code)
                                        .Where(x => x.Instruction.Operand == firstInst.Instruction.Operand)
                                        .Where(x => CodeGroups.AllOpcodes.Where(y => y.StackBehaviourPop != StackBehaviour.Pop0).Select(y => y.Code)
                                                    .Contains(x.Instruction.OpCode.Code))
                                        .Where(x => !new[] { Code.Ret }.Concat(CodeGroups.CallCodes).Contains(x.Instruction.OpCode.Code));
                    var firstInstBackRelated = firstInst.DataFlowBackRelated.Where(x => x.Argument != firstInst);
                    foreach (var secondInstOption in secondInstOptions.ToArray())
                    {
                        var secondInstBackRelated = secondInstOption.DataFlowBackRelated.Where(x => x.Argument != secondInstOption);
                        if (CoupledIndexedArgList.SequenceEqualsWithIndexes(secondInstBackRelated,firstInstBackRelated) && firstInst.DataFlowBackRelated.SelfFeeding == secondInstOption.DataFlowBackRelated.SelfFeeding)
                        {
                            MergeNodes(new[] { firstInst, secondInstOption });
                            mergesWereDone = true;
                            break;
                        }
                    }
                }
            } while (mergesWereDone);
        }

        private void RemoveHelperCodes()
        {
            RemoveInstWrappers(InstructionNodes.Where(x => x is ConstructorCallNode));
            RemoveInstWrappers(InstructionNodes.Where(x => CodeGroups.StLocCodes.Contains(x.Instruction.OpCode.Code)));
            RemoveInstWrappers(InstructionNodes.Where(x => CodeGroups.LdLocCodes.Contains(x.Instruction.OpCode.Code)));
            RemoveInstWrappers(InstructionNodes.Where(x => new[] { Code.Starg, Code.Starg_S }.Contains(x.Instruction.OpCode.Code)));
            //RemoveInstWrappers(InstructionNodes.Where(x => x is StIndInstructionNode && ((StIndInstructionNode) x).AddressType == AddressType.LocalVar));
            RemoveInstWrappers(InstructionNodes.Where(x => x.Instruction.OpCode.Code == Code.Dup));
            RemoveInstWrappers(InstructionNodes.Where(x => x.InliningProperties.Inlined && x is LdArgInstructionNode && x.DataFlowBackRelated.Count > 0 && !x.DataFlowBackRelated.SelfFeeding));
            InstructionNodes.Where(x => x is InlineableCallNode).ToList().ForEach(x => { x.SelfRemove(); InstructionNodes.Remove(x); });
            RemoveInstWrappers(InstructionNodes.Where(x => x is RetInstructionNode && x.InliningProperties.Inlined && !x.DataFlowBackRelated.SelfFeeding));
        }

        private void AddZeroNode()
        {
            var inst = Instruction.Create(CodeGroups.AllOpcodes.First(x => x.Code == Code.Nop));
            InstructionNode nodeZero = _InstructionNodeFactory.GetSingleInstructionNode(inst, metDef);

            foreach (var firstNode in InstructionNodes.Where(x => x.DataFlowBackRelated.Count == 0))
            {
                firstNode.DataFlowBackRelated.AddTwoWay(nodeZero, -1);
            }
            var firstOrderLdArgs = GetFirstOrderLdArgs();
            foreach (var firstOrderLdArg in firstOrderLdArgs)
            {
                if (!firstOrderLdArg.DataFlowBackRelated.Any(x => x.Argument == nodeZero))
                {
                    firstOrderLdArg.DataFlowBackRelated.AddTwoWay(nodeZero, -1);
                }
            }
            InstructionNodes[0].ProgramFlowBackRoutes.AddTwoWay(nodeZero);
            InstructionNodes.Insert(0,nodeZero);
            SetInstructionIndexes();
        }

        private IEnumerable<LdArgInstructionNode> GetFirstOrderLdArgs()
        {
            return InstructionNodes.Where(x => x is LdArgInstructionNode && x.Method == InstructionNodes[0].Method)
                                                                            .Cast<LdArgInstructionNode>()
                                                                            .GroupBy(x => x.ArgIndex)
                                                                            .Select(x => x.OrderBy(y => y.InstructionIndex).First());
        }

        private void MergeImmediateValue()
        {
            foreach (var imeddiateValueNodes in InstructionNodes.Where(x => x is LdImmediateInstNode).Cast<LdImmediateInstNode>().GroupBy(x => x.ImmediateIntValue).Where(x => x.Count()>1).ToList())
            {
                MergeNodes(imeddiateValueNodes);
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
            var ldArgGroups = InstructionNodes.Where(x => x is LdArgInstructionNode)
                                                 .Cast<FunctionArgNodeBase>()
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
            _inlineCallModifier.InlineCallNodes(InstructionNodes);
        }

        private void Verify()
        {
            
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
            if (!nodesToMerge.Any())
            {
                return;
            }
            var nodeToKeep = nodesToMerge.First();
            foreach (var nodeToRemove in nodesToMerge.ToArray().Except(new[] { nodeToKeep }))
            {
                nodeToRemove.MergeInto(nodeToKeep,false);
                InstructionNodes.Remove(nodeToRemove);
                var stillPointingToRemoved = InstructionNodes.Where(x => x.DataFlowBackRelated.Any(y => y.Argument == nodeToRemove)
                                              || x.DataFlowForwardRelated.Any(y => y.Argument == nodeToRemove)
                                              || x.ProgramFlowBackRoutes.Any(y => y == nodeToRemove)
                                              || x.ProgramFlowForwardRoutes.Any(y => y == nodeToRemove)
                                              || x.ProgramFlowBackAffected.Any(y => y.Argument == nodeToRemove)
                                              || x.ProgramFlowForwardAffecting.Any(y => y.Argument == nodeToRemove))
                                              .ToList();
                if (stillPointingToRemoved.Count > 0)
                {
                    throw new Exception("there's someone still pointing to the rmoeved");
                }
            }
            SetInstructionIndexes();
            //Verify();
        }

        public void RemoveInstWrappers(IEnumerable<InstructionNode> instsToRemove)
        {
            foreach (var nodeToRemove in instsToRemove.ToArray())
            {
                nodeToRemove.SelfRemoveAndStitch();
                var stillPointingToVirt = InstructionNodes.Where(x => x.DataFlowForwardRelated.Any(y => y.Argument is VirtualCallInstructionNode)).ToArray();
                //Verify();
                InstructionNodes.Remove(nodeToRemove);
                var stillPointingToRemoved = InstructionNodes.Where(x => x.DataFlowBackRelated.Any(y => y.Argument == nodeToRemove)
                                               || x.DataFlowForwardRelated.Any(y => y.Argument == nodeToRemove)
                                               || x.ProgramFlowBackRoutes.Any(y => y == nodeToRemove)
                                               || x.ProgramFlowForwardRoutes.Any(y => y == nodeToRemove)).ToList();
                if (stillPointingToRemoved.Count() >0)
                {
                    throw new Exception("there's someone still pointing to the removed");
                }
            }
            SetInstructionIndexes();
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

        public List<InstructionNode> FullRunForTesting()
        {
            bool shouldRerun = true;
            bool isFirstRun = true;
            SetInstructionIndexes();
            while (shouldRerun)
            {
                _programFlowManager.AddFlowConnections(InstructionNodes);
                if (isFirstRun)
                {
                    _backTraceManager.DataTraceInFunctionBounds(InstructionNodes);
                }
                InlineFunctionCalls();
                SetInstructionIndexes();
                _backTraceManager.BackTraceOutsideFunctionBounds(InstructionNodes);
                RemoveHelperCodes();
                //MergeSingleOperationNodes();
                MergeSimilarInstructions();
                RecursionFix();
                ResolveVirtualMethods(out shouldRerun);
                SetInstructionIndexes();
                isFirstRun = false;
            }
            RemoveHelperCodes();
            AddZeroNode();
            Verify();

            return InstructionNodes;
        }

    }
}