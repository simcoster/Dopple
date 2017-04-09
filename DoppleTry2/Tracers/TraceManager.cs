using Dopple.BackTracers;
using Dopple.InstructionNodes;
using Dopple.Tracers.DynamicTracing;
using Dopple.Tracers.PredciateProviders;
using Dopple.VerifierNs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.BackTracers
{
    public class TraceManager
    {
        public TraceManager()
        {
            _InFuncDataTransferBackTracers = new BackTracer[] { _LdArgBacktracer, _LdLocBackTracer, _RetBackTracer };

            _OutFuncDataTransferBackTracers = new BackTracer[]{ _LdStaticFieldBackTracer, _LdStaticFieldBackTracer 
                            ,_LoadMemoryByOperandBackTracer ,_TypedReferenceBackTracer,_LdElemBacktracer, _LdFldBacktracer};
        }
        private readonly Verifier[] verifiers;

        private readonly StackForwardTracer _StackForwardTracer = new StackForwardTracer();
        private readonly LdArgBacktracer _LdArgBacktracer = new LdArgBacktracer();
        private readonly LdLocBackTracer _LdLocBackTracer = new LdLocBackTracer();
        private readonly RetBackTracer _RetBackTracer = new RetBackTracer();

        internal void DataTraceInFunctionBounds(List<InstructionNode> instructionNodes)
        {
            //Stopwatch stopwatch = Stopwatch.StartNew();
            _StackForwardTracer.TraceForward(instructionNodes);
            //Console.WriteLine("Stack forward took" + stopwatch.Elapsed);
            //stopwatch.Reset();
            //Console.WriteLine("conditional tool forward took" + stopwatch.Elapsed);
            //stopwatch.Reset();
            TraceDataTransferingNodeRec(instructionNodes[0], _InFuncDataTransferBackTracers);
            //Console.WriteLine("Data transfer took" + stopwatch.Elapsed);
            //stopwatch.Reset();
        }

        BackTracer[] _InFuncDataTransferBackTracers;
      

        private readonly LdStaticFieldBackTracer _LdStaticFieldBackTracer = new LdStaticFieldBackTracer();
        private readonly LindBacktracer _LoadMemoryByOperandBackTracer = new LindBacktracer();
        private readonly TypedReferenceBackTracer _TypedReferenceBackTracer = new TypedReferenceBackTracer();
        private readonly ConditionionalsTracer _ConditionalBacktracer = new ConditionionalsTracer();
        private readonly LdElemBacktracer _LdElemBacktracer = new LdElemBacktracer();
        private readonly LdFldBacktracer _LdFldBacktracer = new LdFldBacktracer();

        public static int CountVisitedNodes = 0;


        internal void BackTraceOutsideFunctionBounds(List<InstructionNode> instructionNodes)
        {
            CountVisitedNodes = 0;
            var mergingNodesData = new Dictionary<InstructionNode, MergeNodeTraceData>();
            foreach (var mergingNode in instructionNodes.Where(x => x.BranchProperties.MergingNodeProperties.IsMergingNode))
            {
                mergingNodesData.Add(mergingNode, new MergeNodeTraceData());
            }
            TraceOutsideFunctionBoundsRec(instructionNodes[0], mergingNodesData);
            var nonPassedThrough = instructionNodes.Except(GlobalVisited).ToArray().OrderBy(x => x.InstructionIndex).ToArray();
            if (nonPassedThrough.Any())
            {
                var wasntReached = instructionNodes.Except(GlobalVisited).ToArray().OrderBy(x => x.InstructionIndex).First();
                Console.WriteLine("Node wasn't reached is " + wasntReached.InstructionIndex + " " + wasntReached.Instruction);
                //throw new Exception("some nodes not reached");
            }
            Console.WriteLine("Visited node count is " + GlobalVisited.Count);
            CountVisitedNodes = 0;
        }

        private BackTracer[] _OutFuncDataTransferBackTracers;

        private List<InstructionNode> GlobalVisited = new List<InstructionNode>();
        private void TraceOutsideFunctionBoundsRec(InstructionNode currentNode, Dictionary<InstructionNode, MergeNodeTraceData> mergingNodesData, InstructionNode lastNode = null, StateProviderCollection stateProviders = null, List<InstructionNode> visited = null )
        {
            if (visited == null)
            {
                visited = new List<InstructionNode>();
                lastNode = currentNode;
                stateProviders = new StateProviderCollection();
                GlobalVisited.Clear();
            }
            while (true)
            {
                if (visited.Contains(currentNode))
                {
                    if (!currentNode.BranchProperties.MergingNodeProperties.IsMergingNode)
                    {
                        return;
                    }
                }
                visited.Add(currentNode);
                GlobalVisited.Add(currentNode);
                bool reachedMergeNodeNotLast;
                int stateProviderCount = stateProviders.Count;
                ActOnCurrentNode(currentNode, mergingNodesData, lastNode, stateProviders, out reachedMergeNodeNotLast);
                if (currentNode is LoadFieldNode)
                {
                    var storeVisited = visited.Where(x => x is StoreFieldNode && ((StoreFieldNode) x).FieldDefinition.Name == ((LoadFieldNode) currentNode).FieldDefinition.Name);
                    if (storeVisited.Any())
                    {

                    }
                }
                if (stateProviders.Count < 2)
                {

                }
                if (reachedMergeNodeNotLast)
                {
                    return;
                }
                lastNode = currentNode;
                if (currentNode.ProgramFlowForwardRoutes.Count == 1)
                {
                    currentNode = currentNode.ProgramFlowForwardRoutes[0];
                }
                else
                {
                    break;
                }
            }
            if (currentNode.ProgramFlowForwardRoutes.Count == 0)
            {
                return;
            }
            if (!(currentNode is ConditionalJumpNode))
            {
                throw new Exception("split without a conditional");
            }
            ConditionalJumpNode nodeAsConditional = (ConditionalJumpNode) currentNode;
            var loopBranches = nodeAsConditional.CreatedBranches.Where(x => x.BranchType == BranchPropertiesNS.BranchType.Loop);
            ProgressNext(mergingNodesData, lastNode, stateProviders, visited, nodeAsConditional, loopBranches);
            var nonLoopBranches = nodeAsConditional.CreatedBranches.Where(x => x.BranchType != BranchPropertiesNS.BranchType.Loop);
            ProgressNext(mergingNodesData, lastNode, stateProviders, visited, nodeAsConditional, nonLoopBranches);
        }

        private void ProgressNext(Dictionary<InstructionNode, MergeNodeTraceData> mergingNodesData, InstructionNode lastNode, StateProviderCollection stateProviders, List<InstructionNode> visited, ConditionalJumpNode currentNode, IEnumerable<BranchPropertiesNS.BranchID> branches)
        {
            foreach (var branch in branches)
            {
                InstructionNode nextNode;
                if (branch.BranchNodes.Count == 0)
                {
                    if (currentNode.ProgramFlowForwardRoutes.Contains(branch.MergingNode))
                    {
                        nextNode = branch.MergingNode;
                    }
                    else
                    {
                        throw new Exception("Can't find next node");
                    }
                }
                else
                {
                    if (!currentNode.ProgramFlowForwardRoutes.Contains(branch.BranchNodes.First()))
                    {
                        throw new Exception("First node in branch is not next");
                    }

                    nextNode = branch.BranchNodes.First();
                }

                TraceOutsideFunctionBoundsRec(nextNode, mergingNodesData, lastNode, stateProviders.Clone(), visited);
            }
        }

        internal void TraceConditionals(List<InstructionNode> instructionNodes)
        {
            _ConditionalBacktracer.TraceConditionals(instructionNodes);
        }

        private static void ActOnCurrentNode(InstructionNode currentNode, Dictionary<InstructionNode, MergeNodeTraceData> mergingNodesData, InstructionNode lastNode, StateProviderCollection stateProviders, out bool reachedMergeNodeNotLast)
        {
            if (currentNode.BranchProperties.MergingNodeProperties.IsMergingNode)
            {
                lock (mergingNodesData)
                {
                    var lastNodeAsConditional = lastNode as ConditionalJumpNode;
                    if (lastNodeAsConditional != null && lastNodeAsConditional.CreatedBranches.Any(x => x.MergingNode == currentNode))
                    {
                        var emptyBranchToMe = lastNodeAsConditional.CreatedBranches.Where(x => x.MergingNode == currentNode && x.BranchNodes.SequenceEqual(new[] { currentNode }));
                        if (emptyBranchToMe.Count() !=1)
                        {
                            throw new Exception("Should only be 1 empty branch to me");
                        }
                        mergingNodesData[currentNode].ReachedBranches.AddDistinct(emptyBranchToMe.First());
                        //TODO if last was conditional and the merging is a call node and was removed need to deal with
                    }
                    else
                    {
                        mergingNodesData[currentNode].ReachedBranches.AddRangeDistinct(lastNode.BranchProperties.Branches);
                        if (!lastNode.BranchProperties.Branches.Any(x => currentNode.BranchProperties.MergingNodeProperties.MergedBranches.Contains(x)))
                        {
                            //throw new Exception("Reached from unmerged branch");
                        }
                    }
                    mergingNodesData[currentNode].AccumelatedStateProviders.AddNewProviders(stateProviders);
                    bool allBranchesReached = !currentNode.BranchProperties.MergingNodeProperties.MergedBranches.Except(mergingNodesData[currentNode].ReachedBranches).Any();
                    if (allBranchesReached)
                    {
                        mergingNodesData[currentNode].AllBranchesReached = true;
                        mergingNodesData[currentNode].AccumelatedStateProviders.MergeBranches(currentNode);
                        stateProviders.AddNewProviders(mergingNodesData[currentNode].AccumelatedStateProviders);
                    }
                    else
                    {
                        reachedMergeNodeNotLast = true;
                        return;
                    }
                }
            }
            var newStoreState = StoreDynamicDataStateProvider.GetMatchingStateProvider(currentNode);
            if (newStoreState != null)
            {
                stateProviders.AddNewProvider(newStoreState);
            }
            IDynamicDataLoadNode loadNode = currentNode as IDynamicDataLoadNode;
            if (loadNode != null)
            {
                foreach (var storeNode in stateProviders.MatchLoadToStore(currentNode))
                {
                    currentNode.DataFlowBackRelated.AddTwoWay(storeNode, loadNode.DataFlowDataProdivderIndex);
                }
            }
            reachedMergeNodeNotLast = false;
        }

        private void TraceDataTransferingNodeRec(InstructionNode instructionNode, IEnumerable<BackTracer> backTracers, List<InstructionNode> visited = null)
        {
            if (visited == null)
            {
                visited = new List<InstructionNode>();
            }
            if (visited.Contains(instructionNode))
            {
                return;
            }
            visited.Add(instructionNode);
            CountVisitedNodes++;
            while (instructionNode.ProgramFlowForwardRoutes.Count < 2)
            {
                var relevantBackTracer = backTracers.FirstOrDefault(x => x.HandlesCodes.Contains(instructionNode.Instruction.OpCode.Code));
                if (relevantBackTracer != null)
                {
                    relevantBackTracer.BackTraceDataFlow(instructionNode);
                }
                if (instructionNode.ProgramFlowForwardRoutes.Count ==0)
                {
                    return;
                }
                instructionNode = instructionNode.ProgramFlowForwardRoutes[0];
                CountVisitedNodes++;

                if (visited.Contains(instructionNode))
                {
                    return;
                }
                visited.Add(instructionNode);
            }
            foreach (var forwardNode in instructionNode.ProgramFlowForwardRoutes)
            {
                TraceDataTransferingNodeRec(forwardNode, backTracers,visited);
            }
        }

       
    }
}
