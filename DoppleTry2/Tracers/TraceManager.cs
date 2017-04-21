using Dopple.BackTracers;
using Dopple.BranchPropertiesNS;
using Dopple.InstructionNodes;
using Dopple.Tracers.DynamicTracing;
using Dopple.Tracers.StateProviders;
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
            _StackForwardTracer.TraceForward(instructionNodes);
            _Conditionalstracer.TraceConditionals(instructionNodes);
            TraceDataTransferingNodeRec(instructionNodes[0], _InFuncDataTransferBackTracers);
        }

        BackTracer[] _InFuncDataTransferBackTracers;
      

        private readonly LdStaticFieldBackTracer _LdStaticFieldBackTracer = new LdStaticFieldBackTracer();
        private readonly LindBacktracer _LoadMemoryByOperandBackTracer = new LindBacktracer();
        private readonly TypedReferenceBackTracer _TypedReferenceBackTracer = new TypedReferenceBackTracer();
        private readonly ConditionionalsTracer _Conditionalstracer = new ConditionionalsTracer();
        private readonly LdElemBacktracer _LdElemBacktracer = new LdElemBacktracer();
        private readonly LdFldBacktracer _LdFldBacktracer = new LdFldBacktracer();

        public static int CountVisitedNodes = 0;


        internal void TraceDynamicData(List<InstructionNode> instructionNodes)
        {
            CountVisitedNodes = 0;
            var mergingNodesData = new Dictionary<InstructionNode, MergeNodeTraceData>();
            foreach (var mergingNode in instructionNodes.Where(x => x.BranchProperties.MergingNodeProperties.IsMergingNode))
            {
                mergingNodesData.Add(mergingNode, new MergeNodeTraceData());
            }
            var visitCount = new Dictionary<InstructionNode, int>();
            instructionNodes.ForEach(x => visitCount.Add(x, 0));
            TraceOutsideFunctionBoundsRec(instructionNodes[0],visitCount, mergingNodesData);
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
            }

            foreach (var node in instructionNodes.Where(x => x is StoreFieldNode).ToArray())
            {
                var nodeAsDynamicLoad = ((IDynamicDataStoreNode) node);
                var dynamicLoadedData = node.DataFlowBackRelated.Where(x => x.ArgIndex == nodeAsDynamicLoad.DataFlowDataProdivderIndex).Select(x => x.Argument);
                foreach (var forwardNode in node.DataFlowForwardRelated)
                {
                    forwardNode.Argument.DataFlowBackRelated.AddTwoWay(dynamicLoadedData, forwardNode.ArgIndex);
                }
            }
        }

        private BackTracer[] _OutFuncDataTransferBackTracers;

        private List<InstructionNode> GlobalVisited = new List<InstructionNode>();
        private void TraceOutsideFunctionBoundsRec(InstructionNode currentNode,  Dictionary<InstructionNode,int> visitCount, Dictionary<InstructionNode, MergeNodeTraceData> mergingNodesData, InstructionNode lastNode = null, StateProviderCollection stateProviders = null)
        {
            if (lastNode == null)
            {
                lastNode = currentNode;
                stateProviders = new StateProviderCollection();
                GlobalVisited.Clear();
            }
            while (true)
            {
                GlobalVisited.Add(currentNode);
                bool reachedMergeNodeNotLast;          
                ActOnCurrentNode(currentNode, mergingNodesData, lastNode, ref stateProviders, out reachedMergeNodeNotLast);
                if (reachedMergeNodeNotLast)
                {
                    return;
                }
                visitCount[currentNode]++;
                if (visitCount[currentNode] > 3 && !currentNode.BranchProperties.MergingNodeProperties.IsMergingNode)
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
                    // a loop
                    var loopBranch = currentNode.ProgramFlowForwardRoutes.Where(x => x.BranchProperties.FirstInLoop).ToList();
                    //if (loopBranch.Count == 0 && currentNode.ProgramFlowForwardRoutes.Any(x => x.BranchProperties.Branches.First().BranchType == BranchPropertiesNS.BranchType.Loop))
                    //{

                    //}
                    if (loopBranch.Count > 1)
                    {
                        throw new Exception("Can't deal with 2 loops from the same place (too many combinations)");
                    }
                    if (loopBranch.Any())
                    {
                        if (visitCount[loopBranch.First()] <2)
                        {
                            currentNode = loopBranch.First();
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        //A split
                        break;
                    }
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
            foreach (var node in currentNode.ProgramFlowForwardRoutes.Except(currentNode.ProgramFlowForwardRoutes.Where(x => x.BranchProperties.FirstInLoop)).ToList())
            {
                TraceOutsideFunctionBoundsRec(node, visitCount, mergingNodesData, currentNode, stateProviders.Clone());
            }
        }

        internal void TraceConditionals(List<InstructionNode> instructionNodes)
        {
            _Conditionalstracer.TraceConditionals(instructionNodes);
        }

        private static void ActOnCurrentNode(InstructionNode currentNode, Dictionary<InstructionNode, MergeNodeTraceData> mergingNodesData, InstructionNode lastNode,ref StateProviderCollection stateProviders, out bool reachedMergeNodeNotLast)
        {
            if (currentNode.BranchProperties.MergingNodeProperties.IsMergingNode)
            {
                lock (mergingNodesData)
                {
                    mergingNodesData[currentNode].ReachedNodes.Add(lastNode);
                    BranchID emptyBranchToMe= null;
                    IEnumerable<BranchID> nonEmptyBranchesToMe = null;
                    if ((lastNode is ConditionalJumpNode))
                    {
                        emptyBranchToMe = ((ConditionalJumpNode) lastNode).CreatedBranches.FirstOrDefault(x => x.BranchNodes.SequenceEqual(new[] { currentNode }));
                    }
                    if (emptyBranchToMe == null)
                    {
                         nonEmptyBranchesToMe = lastNode.BranchProperties.Branches.Intersect(currentNode.BranchProperties.MergingNodeProperties.MergedBranches);
                    }
                    if (emptyBranchToMe != null || nonEmptyBranchesToMe.Any())
                    {
                        mergingNodesData[currentNode].AccumelatedStateProviders.AddRange(stateProviders.ToList());
                        var mergedBrachesNonEmpty = currentNode.ProgramFlowBackRoutes.Where(x => x.BranchProperties.Branches.Any(y => currentNode.BranchProperties.MergingNodeProperties.MergedBranches.Contains(y)));
                        var mergedBrachesEmpty = currentNode.ProgramFlowBackRoutes.Where(x => x is ConditionalJumpNode).Cast<ConditionalJumpNode>().Where(x => x.CreatedBranches.Intersect(currentNode.BranchProperties.MergingNodeProperties.MergedBranches).Any());
                        var splitMergeMergedBranches = mergedBrachesEmpty.Concat(mergedBrachesNonEmpty).ToList();
                        bool allBranchesReached = !splitMergeMergedBranches.Except(mergingNodesData[currentNode].ReachedNodes).Any();
                        if (allBranchesReached)
                        {
                            stateProviders = new StateProviderCollection(mergingNodesData[currentNode].AccumelatedStateProviders);
                            mergingNodesData[currentNode].AccumelatedStateProviders.Clear();
                            //prepare for next run (for loops)
                            mergingNodesData[currentNode].ReachedNodes.Clear();
                        }
                        else
                        {
                            reachedMergeNodeNotLast = true;
                            return;
                        }
                    }
                    else
                    {

                    }
                }
            }
            var newStoreStateProviders = StoreDynamicDataStateProvider.GetMatchingStateProvider(currentNode);
            if (newStoreStateProviders != null)
            {
                foreach(var newStateProvider in newStoreStateProviders)
                {
                    stateProviders.AddNewProvider(newStateProvider);
                }
            }
            IDynamicDataLoadNode loadNode = currentNode as IDynamicDataLoadNode;
            if (loadNode != null)
            {
                foreach (var stateProvider in stateProviders.MatchLoadToStore(currentNode))
                {
                    stateProvider.ConnectToLoadNode((InstructionNode)loadNode);
                    //have to change this to abstract
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
