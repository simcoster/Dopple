using Dopple.BackTracers;
using Dopple.InstructionNodes;
using Dopple.Tracers.PredciateProviders;
using Dopple.VerifierNs;
using System;
using System.Collections.Generic;
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
            _ConditionalBacktracer.TraceConditionals(instructionNodes);
            TraceDataTransferingNodeRec(instructionNodes[0], _InFuncDataTransferBackTracers);
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
            BackTraceOutsideFunctionBoundsRec(instructionNodes[0], mergingNodesData);
            Console.WriteLine("Visited node count is " + CountVisitedNodes);
            CountVisitedNodes = 0;
        }

        private List<StoreDynamicDataPredicateProvider> DynamicStorePredicateProviders = new List<StoreDynamicDataPredicateProvider>()
        {
            new StFldPredicateProvider(), new StElemPredicateProvider()
        };
        private BackTracer[] _OutFuncDataTransferBackTracers;

        private void BackTraceOutsideFunctionBoundsRec(InstructionNode currentNode, Dictionary<InstructionNode, MergeNodeTraceData> mergingNodesData, InstructionNode lastNode = null, List<PredicateAndNode> predicatesAndNodes =null, List<InstructionNode> visited = null )
        {
            if (visited == null)
            {
                visited = new List<InstructionNode>();
                predicatesAndNodes = new List<PredicateAndNode>();
                lastNode = currentNode;
            }
            if (visited.Contains(currentNode))
            {
                return;
            }
            if (currentNode.BranchProperties.MergingNodeProperties.IsMergingNode)
            {
                mergingNodesData[currentNode].ReachedBranches.AddRange(lastNode.BranchProperties.Branches);
                mergingNodesData[currentNode].AccumelatedPredicates.AddRange(predicatesAndNodes);
                bool allBranchesReached = !currentNode.BranchProperties.MergingNodeProperties.MergedBranches.Except(mergingNodesData[currentNode].ReachedBranches).Any();
                if (!allBranchesReached)
                {
                    return;
                }
                predicatesAndNodes.AddRange(mergingNodesData[currentNode].AccumelatedPredicates);
            }
            var predicateProvider = DynamicStorePredicateProviders.FirstOrDefault(x => x.IsRelevant(currentNode));
            if (predicateProvider != null)
            {
                predicatesAndNodes.Add(predicateProvider.GetMatchingLoadPredicate(currentNode));
            }
            else
            {
                foreach(var predicateAndNode in predicatesAndNodes)
                {
                    if (predicateAndNode.Predicate(currentNode))
                    {
                        int dataTransferIndex = ((IDynamicDataLoadNode) currentNode).DataFlowDataProdivderIndex;
                    }
                }
            }
            lastNode = currentNode;
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
