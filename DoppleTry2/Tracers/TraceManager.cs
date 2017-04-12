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

        public void ForwardDynamicData(List<InstructionNode> instructionNodes)
        {
            foreach(var node in instructionNodes.Where(x => x is LoadFieldNode).ToArray())
            {
                var nodeAsDynamicLoad = ((IDynamicDataLoadNode) node);
                var dynamicLoadedData = node.DataFlowBackRelated.Where(x => x.ArgIndex == nodeAsDynamicLoad.DataFlowDataProdivderIndex).Select(x => x.Argument);
                foreach(var forwardNode in node.DataFlowForwardRelated)
                {
                    forwardNode.Argument.DataFlowBackRelated.AddTwoWay(dynamicLoadedData, forwardNode.ArgIndex);
                }
                node.DataFlowBackRelated.RemoveAllTwoWay(x => x.ArgIndex == nodeAsDynamicLoad.DataFlowDataProdivderIndex);
                //node.DataFlowForwardRelated.RemoveAllTwoWay(x => x.ArgIndex == nodeAsDynamicLoad.DataFlowDataProdivderIndex);
                //node.SelfRemove();
                //instructionNodes.Remove(node);
            }

            foreach (var node in instructionNodes.Where(x => x is StoreFieldNode).ToArray())
            {
                var nodeAsDynamicLoad = ((IDynamicDataStoreNode) node);
                var dynamicLoadedData = node.DataFlowBackRelated.Where(x => x.ArgIndex == nodeAsDynamicLoad.DataFlowDataProdivderIndex).Select(x => x.Argument);
                foreach (var forwardNode in node.DataFlowForwardRelated)
                {
                    forwardNode.Argument.DataFlowBackRelated.AddTwoWay(dynamicLoadedData, forwardNode.ArgIndex);
                }
                node.DataFlowBackRelated.RemoveAllTwoWay(x => x.ArgIndex == nodeAsDynamicLoad.DataFlowDataProdivderIndex);
                node.DataFlowForwardRelated.RemoveAllTwoWay(x => x.ArgIndex == nodeAsDynamicLoad.DataFlowDataProdivderIndex);
                //node.SelfRemove();
                //instructionNodes.Remove(node);

            }
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
            var passedLoopStateProivders = stateProviders.Clone();
            var loopNodes = currentNode.ProgramFlowForwardRoutes.Where(x => x.BranchProperties.FirstInLoopOf != null && x.BranchProperties.FirstInLoopOf.BranchType == BranchPropertiesNS.BranchType.Loop).ToList();
            if (loopNodes.Count >1)
            {
                throw new Exception("More than one originating loops");
            }
            if (loopNodes.Count >0)
            {
                var firstPassLoopStateProviders = new StateProviderCollection();
                foreach(var loopNode in loopNodes)
                {
                    var stateProvidersLoopClone = stateProviders.Clone();
                    TraceOutsideFunctionBoundsRec(loopNodes[0], mergingNodesData, currentNode, stateProvidersLoopClone, visited);
                    firstPassLoopStateProviders.AddNewProviders(stateProvidersLoopClone);
                }

                foreach (var loopNode in loopNodes)
                {
                    var secondPassStateProvidersClone = firstPassLoopStateProviders.Clone();
                    TraceOutsideFunctionBoundsRec(loopNodes[0], mergingNodesData, currentNode, secondPassStateProvidersClone, visited);
                    passedLoopStateProivders.AddNewProviders(secondPassStateProvidersClone);
                }
            }
            foreach (var node in currentNode.ProgramFlowForwardRoutes.Except(loopNodes))
            {
                TraceOutsideFunctionBoundsRec(node, mergingNodesData, currentNode, passedLoopStateProivders.Clone(), visited);
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
                    mergingNodesData[currentNode].ReachedNodes.Add(lastNode);
                    mergingNodesData[currentNode].AccumelatedStateProviders.AddNewProviders(stateProviders);
                    bool allBranchesReached = !currentNode.ProgramFlowBackRoutes.Except(mergingNodesData[currentNode].ReachedNodes).Any();
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
