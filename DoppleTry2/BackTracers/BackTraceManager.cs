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
        BackTracer[] _BackTracers;
        public BackTraceManager()
        {
            _BackTracers = new BackTracer[]{ _LdStaticFieldBackTracer, _LdStaticFieldBackTracer , _LoadFieldByStackBackTracer
                            ,_LoadMemoryByOperandBackTracer ,_TypedReferenceBackTracer, _LdArgBacktracer, _StIndAddressBackTracer };
        }
        private readonly Verifier[] verifiers;

        private readonly StackForwardTracer _StackForwardTracer = new StackForwardTracer();
        private readonly LdArgBacktracer _LdArgBacktracer = new LdArgBacktracer();
        private readonly StIndAddressBackTracer _StIndAddressBackTracer = new StIndAddressBackTracer();
        private readonly LdLocBackTracer _LdLocBackTracer = new LdLocBackTracer();
        private readonly RetBackTracer _RetBackTracer = new RetBackTracer();

        internal void DataTraceInFunctionBounds(List<InstructionNode> instructionNodes)
        {
            _StackForwardTracer.TraceForward(instructionNodes);
            _LdArgBacktracer.AddBackDataflowConnections(instructionNodes);
            _StIndAddressBackTracer.AddBackDataflowConnections(instructionNodes);
            _LdLocBackTracer.AddBackDataflowConnections(instructionNodes);
            _ConditionalBacktracer.TraceConditionals(instructionNodes);
        }

        private readonly LdStaticFieldBackTracer _LdStaticFieldBackTracer = new LdStaticFieldBackTracer();
        private readonly LoadFieldByStackBackTracer _LoadFieldByStackBackTracer = new LoadFieldByStackBackTracer();
        private readonly LoadMemoryByOperandBackTracer _LoadMemoryByOperandBackTracer = new LoadMemoryByOperandBackTracer();
        private readonly TypedReferenceBackTracer _TypedReferenceBackTracer = new TypedReferenceBackTracer();
        private readonly ConditionionalsTracer _ConditionalBacktracer = new ConditionionalsTracer();

        internal void BackTraceOutsideFunctionBounds(List<InstructionNode> instructionNodes)
        {
            _RetBackTracer.AddBackDataflowConnections(instructionNodes);
            _LdStaticFieldBackTracer.AddBackDataflowConnections(instructionNodes);
            _LoadFieldByStackBackTracer.AddBackDataflowConnections(instructionNodes);
            _LoadMemoryByOperandBackTracer.AddBackDataflowConnections(instructionNodes);
            _TypedReferenceBackTracer.AddBackDataflowConnections(instructionNodes);
        }


        public void BackTraceSingleNode(InstructionNode instructionNode)
        {
            var relevantBackTracer = _BackTracers.First(x => x.HandlesCodes.Contains(instructionNode.Instruction.OpCode.Code));
            relevantBackTracer.BackTraceDataFlowSingle(instructionNode);
        }
    }
}
