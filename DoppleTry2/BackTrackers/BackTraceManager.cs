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
        private readonly BackTracer[] backTracers;
        private readonly Verifier[] verifiers;
        private readonly StackForwardTracer _StackForwardTracer = new StackForwardTracer();
        private readonly LdArgBacktracer _LdArgBacktracer = new LdArgBacktracer();

        public BackTraceManager(List<InstructionNode> instructionNodes)
        {
            backTracers =
                           new BackTracer[]
                           {
                            new LdStaticFieldBackTracer(),
                            new LoadFieldByStackBackTracer(),
                            new LoadMemoryByOperandBackTracer(),
                            new TypedReferenceBackTracer(),
                            new LdLocBackTracer(),
                            new ConstructorReturnBackTracer(),
                           };
            verifiers = new Verifier[] {new StElemVerifier(instructionNodes), new StackPopPushVerfier(instructionNodes),
                                            new TwoWayVerifier(instructionNodes), new ArithmeticsVerifier(instructionNodes),
                                            new ArgIndexVerifier(instructionNodes), new LdElemVerifier(instructionNodes) };
        }

        public void BackTrace(InstructionNode instructionNode)
        {
            foreach (var backTracer in backTracers.Where(x => x.HandlesCodes.Contains(instructionNode.Instruction.OpCode.Code)))
            {
                backTracer.AddBackDataflowConnections(instructionNode);
                foreach (var verifier in verifiers)
                {
                    verifier.Verify(instructionNode);
                }
            }
        }
        public IEnumerable<BackTracer> GetRelevantBackTracers(InstructionNode instructionNode)
        {
            return backTracers.Where(x => x.HandlesCodes.Contains(instructionNode.Instruction.OpCode.Code));
        }

        internal void BackTraceInFunctionBounds(List<InstructionNode> instructionNodes)
        {
            _StackForwardTracer.TraceForward(instructionNodes);
            foreach (var instWrapper in instructionNodes.Where(x => x is LdArgInstructionNode).OrderByDescending(x => x.InstructionIndex))
            {
                _LdArgBacktracer.AddBackDataflowConnections(instWrapper);
            }
            var stIndAddressBackTracer = new StIndAddressBackTracer();
            foreach (var instWrapper in instructionNodes.Where(x => stIndAddressBackTracer.HandlesCodes.Contains(x.Instruction.OpCode.Code)).OrderByDescending(x => x.InstructionIndex))
            {
                stIndAddressBackTracer.AddBackDataflowConnections(instWrapper);
            }
        }

        internal void BackTraceOutsideFunctionBounds(List<InstructionNode> instructionNodes)
        {
            foreach (var node in instructionNodes.OrderByDescending(x => x.InstructionIndex))
            {
                BackTrace(node);
            }
        }
    }
}
