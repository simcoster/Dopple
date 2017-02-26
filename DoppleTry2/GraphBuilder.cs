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
        private readonly BackTraceManager _backTraceManager;
        private InstructionNodeFactory _InstructionNodeFactory = new InstructionNodeFactory();
        private InlineCallModifier _inlineCallModifier;

        public GraphBuilder(IEnumerable<InstructionNode> instNodes)
        {
            InstructionNodes = instNodes.ToList();
            _inlineCallModifier = new InlineCallModifier(_InstructionNodeFactory);
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

            _postBacktraceModifiers = new IPostBackTraceModifier[] { };

            _backTraceManager = new BackTraceManager(InstructionNodes);
            _inlineCallModifier = new InlineCallModifier(_InstructionNodeFactory);
        }


        public List<InstructionNode> Run()
        {
            SetInstructionIndexes();
            _programFlowManager.AddFlowConnections(InstructionNodes);
            InlineFunctionCalls();
            SetInstructionIndexes();
            try
            {
                BackTrace();
            }
            catch(StackPopException stackPopException)
            {
                return stackPopException.problematicRoute;
            }
            RemoveHelperCodes();
            //RecursionFix();
            //MergeSingleOperationNodes();
            BackTraceConditionals();
            MergeSimilarInstructions();
            LdElemBackTrace();
            AddZeroNode();
            SetInstructionIndexes();
            //Verify();

            return InstructionNodes;
        }

        private void BackTraceConditionals()
        {
            ConditionionalsBackTracer conditionalBacktracer = new ConditionionalsBackTracer(InstructionNodes);
            foreach(var condNode in InstructionNodes.Where(x => x is ConditionalJumpNode))
            {
                conditionalBacktracer.AddBackDataflowConnections(condNode);
            }
        }

        private void RecursionFix()
        {
            foreach(var inlinedCallNode in InstructionNodes.Where(x => x is InlineableCallNode && !(x is ConstructorCallNode)).ToArray())
            {
                inlinedCallNode.DataFlowForwardRelated.RemoveAllTwoWay();
                inlinedCallNode.DataFlowBackRelated.RemoveAllTwoWay();
                inlinedCallNode.ProgramFlowBackAffected.RemoveAllTwoWay();
                inlinedCallNode.ProgramFlowForwardAffecting.RemoveAllTwoWay();
                foreach(var forwardRoute in inlinedCallNode.ProgramFlowForwardRoutes)
                {
                    forwardRoute.ProgramFlowBackRoutes.AddTwoWay(inlinedCallNode.ProgramFlowBackRoutes);
                }
                inlinedCallNode.ProgramFlowBackRoutes.RemoveAllTwoWay();
                inlinedCallNode.ProgramFlowForwardRoutes.RemoveAllTwoWay();
                InstructionNodes.Remove(inlinedCallNode);
            }

            //need to fix this so it only applies to recursive node instances
            return;
            foreach(var recusriveMethodNodesGroup in InstructionNodes.GroupBy(x => new { x.Method, x.Instruction.Offset }).Where(x => x.Count() >1).ToList())
            {
                if (recusriveMethodNodesGroup.Count(x => !x.InliningProperties.Recursive) != 1)
                {
                    throw new Exception("only one non recursive instance should exist");
                }
                var nonRecursiveNode = recusriveMethodNodesGroup.First(x => !x.InliningProperties.Recursive);
                foreach (var recursiveNode in recusriveMethodNodesGroup.Where(x => x.InliningProperties.Recursive))
                {
                    recursiveNode.MergeInto(nonRecursiveNode);
                    InstructionNodes.Remove(recursiveNode);
                }
            }
        }

        private void MergeSingleOperationNodes()
        {
            var postMergeBackTracers = new BackTracer[] {
                                                        new SingleConditionOperationUnitBackTracer(InstructionNodes),
                                                        new SingleArithmeticWithConstantBacktracer(InstructionNodes)
                                                        };
            foreach (var node in InstructionNodes.OrderByDescending(x => x.InstructionIndex))
            {
                foreach (var backTracer in postMergeBackTracers)
                {
                    if (backTracer.HandlesCodes.Contains(node.Instruction.OpCode.Code))
                    {
                        backTracer.AddBackDataflowConnections(node);
                    }
                }
            }

            var blah = InstructionNodes.Where(x => x.SingleUnitBackRelated.Count > 0);
            var frontMostNodes = InstructionNodes.Where(x => x.SingleUnitBackRelated.Count > 0 && x.SingleUnitForwardRelated.Count == 0);
            foreach (var frontMostNode in frontMostNodes.ToArray())
            {
                Queue<InstructionNode> singleUnitBackNodes = new Queue<InstructionNode>();
                frontMostNode.SingleUnitBackRelated.ForEach(x => singleUnitBackNodes.Enqueue(x));
                while(singleUnitBackNodes.Count >0)
                {
                    var currNode = singleUnitBackNodes.Dequeue();
                    currNode.SingleUnitBackRelated.ForEach(x => singleUnitBackNodes.Enqueue(x));
                    currNode.MergeInto(frontMostNode);
                    InstructionNodes.Remove(currNode);
                    frontMostNode.SingleUnitNodes.Add(currNode);
                }
            }
        }

        private void LdElemBackTrace()
        {
            var postMergeBackTracers = new BackTracer[] {
                                                        new LdElemBacktracer(InstructionNodes),
                                                        new LdFldBacktracer(InstructionNodes)
                                                        };
            foreach (var node in InstructionNodes.OrderByDescending(x => x.InstructionIndex))
            {
                foreach (var backTracer in postMergeBackTracers)
                {
                    if (backTracer.HandlesCodes.Contains(node.Instruction.OpCode.Code))
                    {
                        backTracer.AddBackDataflowConnections(node);
                    }
                }
            }
        }

        private void MergeSimilarInstructions()
        {
            MergeLdArgs();
            MergeImmediateValue();
            MergeLoadNulls();
            MergeEquivilentPairs();
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
                                        .Where(x => typeof(OpCodes).GetFields().Select(y => y.GetValue(null))
                                                    .Cast<OpCode>().Where(y => y.StackBehaviourPop != StackBehaviour.Pop0).Select(y => y.Code)
                                                    .Contains(x.Instruction.OpCode.Code))
                                        .Where(x => !new[] { Code.Ret }.Concat(CodeGroups.CallCodes).Contains(x.Instruction.OpCode.Code));
                    var firstInstBackRelated = firstInst.DataFlowBackRelated.Where(x => x.Argument != firstInst);
                    foreach (var secondInstOption in secondInstOptions.ToArray())
                    {
                        var secondInstBackRelated = secondInstOption.DataFlowBackRelated.Where(x => x.Argument != secondInstOption);
                        if (ArgList.SequenceEqualsWithIndexes(secondInstBackRelated,firstInstBackRelated) && firstInst.DataFlowBackRelated.SelfFeeding == secondInstOption.DataFlowBackRelated.SelfFeeding)
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
            RemoveInstWrappers(InstructionNodes.Where(x => CodeGroups.StLocCodes.Contains(x.Instruction.OpCode.Code)));
            RemoveInstWrappers(InstructionNodes.Where(x => CodeGroups.LdLocCodes.Contains(x.Instruction.OpCode.Code)));
            RemoveInstWrappers(InstructionNodes.Where(x => new[] { Code.Starg, Code.Starg_S }.Contains(x.Instruction.OpCode.Code)));
            RemoveInstWrappers(InstructionNodes.Where(x => x is LdArgInstructionNode && x.DataFlowBackRelated.Count >0 && !x.DataFlowBackRelated.SelfFeeding));
            RemoveInstWrappers(InstructionNodes.Where(x => x is StIndInstructionNode && ((StIndInstructionNode) x).AddressType == AddressType.LocalVar));
            //TODO check this
            RemoveInstWrappers(InstructionNodes.Where(x => x.Instruction.OpCode.Code == Code.Ret && x.InliningProperties.Inlined && !x.DataFlowBackRelated.SelfFeeding));
            RemoveInstWrappers(InstructionNodes.Where(x => x.Instruction.OpCode.Code == Code.Dup));
        }

        private void AddZeroNode()
        {
            var inst = Instruction.Create(typeof(OpCodes).GetFields().Select(x => x.GetValue(null)).Cast<OpCode>().First(x => x.Code == Code.Nop));
            InstructionNode nodeZero = _InstructionNodeFactory.GetSingleInstructionNode(inst, metDef);

            foreach (var firstNode in InstructionNodes.Where(x => x.DataFlowBackRelated.Count == 0))
            {
                firstNode.DataFlowBackRelated.AddTwoWay(nodeZero);
            }
            var firstOrderLdArgs = GetFirstOrderLdArgs();
            foreach (var firstOrderLdArg in firstOrderLdArgs)
            {
                if (!firstOrderLdArg.DataFlowBackRelated.Any(x => x.Argument == nodeZero))
                {
                    firstOrderLdArg.DataFlowBackRelated.AddTwoWay(nodeZero);
                }
            }
            InstructionNodes[0].ProgramFlowBackRoutes.AddTwoWay(nodeZero);
            InstructionNodes.Add(nodeZero);
            SetInstructionIndexes();
        }

        private IEnumerable<LdArgInstructionNode> GetFirstOrderLdArgs()
        {
            return InstructionNodes.Where(x => x is LdArgInstructionNode && x.Method == InstructionNodes[0].Method)
                                                                            .Cast<LdArgInstructionNode>()
                                                                            .GroupBy(x => x.ArgIndex)
                                                                            .Select(x => x.OrderBy(y => y.InstructionIndex).First());
        }

        private void BackTrace()
        {
            new StackForwardTracer(InstructionNodes).TraceForward(InstructionNodes[0]);
            foreach (var instWrapper in InstructionNodes.Where(x => x is LdArgInstructionNode).OrderByDescending(x => x.InstructionIndex))
            {
                _backTraceManager.BackTrace(instWrapper);
            }
            var stIndAddressBackTracer = new StIndAddressBackTracer(InstructionNodes);
            foreach (var instWrapper in InstructionNodes.Where(x => stIndAddressBackTracer.HandlesCodes.Contains(x.Instruction.OpCode.Code)).OrderByDescending(x => x.InstructionIndex))
            {
                stIndAddressBackTracer.AddBackDataflowConnections(instWrapper);
            }
            foreach (var node in InstructionNodes.OrderByDescending(x => x.InstructionIndex))
            {
                _backTraceManager.BackTrace(node);
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
            _inlineCallModifier.Modify(InstructionNodes);
        }

        private void Verify()
        {
            var verifiers = new Verifier[] {new StElemVerifier(InstructionNodes), new StackPopPushVerfier(InstructionNodes),
                                            new TwoWayVerifier(InstructionNodes), new ArithmeticsVerifier(InstructionNodes),
                                            new ArgIndexVerifier(InstructionNodes), new LdElemVerifier(InstructionNodes),
                                            new ArgRemovedVerifier(InstructionNodes)};
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
            var nodeToKeep = nodesToMerge.First();
            foreach (var nodeToRemove in nodesToMerge.ToArray().Except(new[] { nodeToKeep }))
            {
                nodeToRemove.MergeInto(nodeToKeep);
                InstructionNodes.Remove(nodeToRemove);
                var stillPointingToRemoved = InstructionNodes.Where(x => x.DataFlowBackRelated.Any(y => y.Argument == nodeToRemove)
                                              || x.DataFlowForwardRelated.Any(y => y.Argument == nodeToRemove)
                                              || x.ProgramFlowBackRoutes.Any(y => y == nodeToRemove)
                                              || x.ProgramFlowForwardRoutes.Any(y => y == nodeToRemove)).ToList();
                if (stillPointingToRemoved.Count > 0)
                {
                    throw new Exception("there's someone still pointing to the rmoeved");
                }
            }
            SetInstructionIndexes();
            //Veirify();
        }

        public void RemoveInstWrappers(IEnumerable<InstructionNode> instsToRemove)
        {
            foreach (var nodeToRemove in instsToRemove.ToArray())
            {
                nodeToRemove.SelfRemove();
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
    }
}