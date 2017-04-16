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
using System.Diagnostics;
using Dopple.BranchPropertiesNS;

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
        private readonly TraceManager _backTraceManager = new TraceManager();
        private InstructionNodeFactory _InstructionNodeFactory = new InstructionNodeFactory();
        private VirtualMethodResolver _VirtualMethodResolver = new VirtualMethodResolver();
        private CallInliner _inlineCallModifier;
        ConditionionalsTracer _ConditionionalsTracer = new ConditionionalsTracer();
        Verifier[] verifiers;

        public GraphBuilder(FunctionFlowGraph functionFlowGraph)
        {
            List<Instruction> instructions = functionFlowGraph.Method.Body.Instructions.ToList();
            if (instructions.Any(x => x.OpCode.Code == Code.Ldsfld))
            {
                //instructions = functionFlowGraph.TypeInitializer.Body.Instructions.Where(x => x.OpCode.Code != Code.Ret).Concat(instructions).ToList();
            }
            InstructionNodes =
                instructions.SelectMany(x => _InstructionNodeFactory.GetInstructionNodes(x, functionFlowGraph.Method)).ToList();
            foreach (var inst in InstructionNodes)
            {
                inst.InstructionIndex = InstructionNodes.IndexOf(inst);
            }
            metDef = functionFlowGraph.Method;
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
            int runCounter = 0;
            bool shouldRerun = true;
            bool isFirstRun = true;
            bool shouldRunDynamicTrace = true;
            BranchProperties.BaseBranch.BranchNodes.AddRange(InstructionNodes);
            SetInstructionIndexes();
            while (shouldRerun)
            {
                Console.WriteLine("run counter is " + runCounter);
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
            
                //if (shouldRunDynamicTrace)
                //{
                //TODO remove for tests;
                BackTraceOutsideFuncBoundry();
              
                // }
                //MergeSingleOperationNodes();
                ResolveVirtualMethods(out shouldRerun, out shouldRunDynamicTrace);
             
                //SetInstructionIndexes();
                isFirstRun = false;
                runCounter++;
            }
            //RecursionFix();
            //RemoveHelperCodes();
            //RemoveAndStitchDynamicDataConnections();

            //_backTraceManager.ForwardDynamicData(InstructionNodes);
            MergeSimilarInstructions();
            //MergeEquivilentPairs();
            AddZeroNode();
            //Verify();
            //its still very slow, need to think of solution to optimize, maybe don't need to run tracing each  time, only when there's danger something will change
            // okay, i'm only going to run it again if it contains stfld, stelem, or stsfld
            
            return InstructionNodes;
        }

        private void RemoveAndStitchDynamicDataConnections()
        {
            DynamicDataConnector.DynamicStoreNodesRewire(InstructionNodes);
            DynamicDataConnector.DynamicLoadNodesRewire(InstructionNodes);
        }

        private void BackTraceOutsideFuncBoundry()
        {
            _backTraceManager.BackTraceOutsideFunctionBounds(InstructionNodes);
        }

        private void ResolveVirtualMethods(out bool inliningWasDone, out bool dynamicStoreInMethods )
        {
            _VirtualMethodResolver.ResolveVirtualMethods(InstructionNodes, out inliningWasDone, out dynamicStoreInMethods);
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
            RemoveAndStitchNodes(InstructionNodes.Where(x => x is ConstructorCallNode));
            RemoveAndStitchNodes(InstructionNodes.Where(x => CodeGroups.StLocCodes.Contains(x.Instruction.OpCode.Code)));
            RemoveAndStitchNodes(InstructionNodes.Where(x => CodeGroups.LdLocCodes.Contains(x.Instruction.OpCode.Code)));
            RemoveAndStitchNodes(InstructionNodes.Where(x => new[] { Code.Starg, Code.Starg_S }.Contains(x.Instruction.OpCode.Code)));
            //RemoveInstWrappers(InstructionNodes.Where(x => x is StIndInstructionNode && ((StIndInstructionNode) x).AddressType == AddressType.LocalVar));
            RemoveAndStitchNodes(InstructionNodes.Where(x => x.Instruction.OpCode.Code == Code.Dup));
            RemoveAndStitchNodes(InstructionNodes.Where(x => x.InliningProperties.Inlined && x is LdArgInstructionNode && x.DataFlowBackRelated.Count > 0 && !x.DataFlowBackRelated.SelfFeeding));
            RemoveAndStitchNodes(InstructionNodes.Where(x => x is RetInstructionNode && x.InliningProperties.Inlined && !x.DataFlowBackRelated.SelfFeeding));
        }

        private void AddZeroNode()
        {
            var inst = Instruction.Create(CodeGroups.AllOpcodes.First(x => x.Code == Code.Nop));
            InstructionNode nodeZero = _InstructionNodeFactory.GetSingleInstructionNode(inst, metDef);

            foreach (var firstNode in InstructionNodes.Where(x => x.DataFlowBackRelated.Count == 0))
            {
                firstNode.DataFlowBackRelated.AddTwoWay(nodeZero, -1);
            }
            foreach (var uninitilizedLoadNode in InstructionNodes.Where(x => x is IDynamicDataLoadNode && !((IDynamicDataLoadNode)x).AllPathsHaveAStoreNode))
            {
                uninitilizedLoadNode.DataFlowBackRelated.AddTwoWay(nodeZero, -1);
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
                                              || x.ProgramFlowForwardRoutes.Any(y => y == nodeToRemove))
                                              .ToList();
                if (stillPointingToRemoved.Count > 0)
                {
                    throw new Exception("there's someone still pointing to the rmoeved");
                }
            }
            SetInstructionIndexes();
            //Verify();
        }

        public void RemoveAndStitchNodes(IEnumerable<InstructionNode> instsToRemove)
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
            bool shouldRunDynamicTrace = true;
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
                if (shouldRunDynamicTrace)
                {
                    _backTraceManager.BackTraceOutsideFunctionBounds(InstructionNodes);
                }
                RemoveHelperCodes();
                //MergeSingleOperationNodes();
                MergeSimilarInstructions();
                RecursionFix();
                ResolveVirtualMethods(out shouldRerun, out shouldRunDynamicTrace);
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