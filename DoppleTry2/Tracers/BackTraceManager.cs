using Dopple.BackTracers;
using Dopple.InstructionNodes;
using Dopple.VerifierNs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.BackTracers
{
    public class BackTraceManager
    {
        public BackTraceManager()
        {
            _InFuncDataTransferBackTracers = new BackTracer[] { _LdArgBacktracer, _LdLocBackTracer, _RetBackTracer };

            _OutFuncDataTransferBackTracers = new BackTracer[]{ _LdStaticFieldBackTracer, _LdStaticFieldBackTracer , _LoadFieldByStackBackTracer
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
        private readonly LoadFieldByStackBackTracer _LoadFieldByStackBackTracer = new LoadFieldByStackBackTracer();
        private readonly LindBacktracer _LoadMemoryByOperandBackTracer = new LindBacktracer();
        private readonly TypedReferenceBackTracer _TypedReferenceBackTracer = new TypedReferenceBackTracer();
        private readonly ConditionionalsTracer _ConditionalBacktracer = new ConditionionalsTracer();

        BackTracer[] _OutFuncDataTransferBackTracers;

        internal void BackTraceOutsideFunctionBounds(List<InstructionNode> instructionNodes)
        {
            TraceDataTransferingNodeRec(instructionNodes[0], _OutFuncDataTransferBackTracers);
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

        LdElemBacktracer _LdElemBacktracer = new LdElemBacktracer();
        LdFldBacktracer _LdFldBacktracer = new LdFldBacktracer();
    }
}
