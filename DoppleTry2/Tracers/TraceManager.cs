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
            _ConditionalBacktracer.TraceConditionals(instructionNodes);
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
            var nonPassedThrough = mergingNodesData.Keys.Where(x => !x.BranchProperties.MergingNodeProperties.MergedBranches.SequenceEqual(mergingNodesData[x].ReachedBranches));
            Console.WriteLine("Visited node count is " + GlobalVisited.Count);
            CountVisitedNodes = 0;
        }

        private BackTracer[] _OutFuncDataTransferBackTracers;

        private List<InstructionNode> GlobalVisited = new List<InstructionNode>();
        private void TraceOutsideFunctionBoundsRec(InstructionNode currentNode, Dictionary<InstructionNode, MergeNodeTraceData> mergingNodesData, InstructionNode lastNode = null, StateProviders stateProviders = null, List<InstructionNode> visited = null )
        {
            if (visited == null)
            {
                visited = new List<InstructionNode>();
                lastNode = currentNode;
                stateProviders = new StateProviders();
                GlobalVisited.Clear();
            }
            while (true)
            {
                if (visited.Contains(currentNode) && !currentNode.BranchProperties.MergingNodeProperties.IsMergingNode)
                {
                    return;
                }
                visited.Add(currentNode);
                GlobalVisited.Add(currentNode);
                bool reachedMergeNodeNotLast;
                ActOnCurrentNode(currentNode, mergingNodesData, lastNode, ref stateProviders, out reachedMergeNodeNotLast);
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
            if (currentNode.ProgramFlowForwardRoutes.Count ==0)
            {

            }
            foreach(var nextNode in currentNode.ProgramFlowForwardRoutes)
            {
                TraceOutsideFunctionBoundsRec(nextNode, mergingNodesData, lastNode, stateProviders.Clone(), visited);
            }
        }

        private static void ActOnCurrentNode(InstructionNode currentNode, Dictionary<InstructionNode, MergeNodeTraceData> mergingNodesData, InstructionNode lastNode, ref StateProviders stateProviders, out bool reachedMergeNodeNotLast)
        {
            if (currentNode.BranchProperties.MergingNodeProperties.IsMergingNode)
            {
                lock (mergingNodesData)
                {
                    ConditionalJumpNode lastAsConditional = lastNode as ConditionalJumpNode;
                    if (lastAsConditional != null)
                    {
                        //TODO if last was conditional and the merging is a call node and was removed need to deal with
                        mergingNodesData[currentNode].ReachedBranches.AddDistinct(lastAsConditional.ForwardBranchedPaths.First(x => x.Item1 == currentNode).Item2);
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
                        mergingNodesData[currentNode].AccumelatedStateProviders.MergeBranches(currentNode);
                        stateProviders = mergingNodesData[currentNode].AccumelatedStateProviders;
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
