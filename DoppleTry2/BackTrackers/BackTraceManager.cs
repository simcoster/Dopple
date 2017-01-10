using DoppleTry2.BackTrackers;
using DoppleTry2.InstructionNodes;
using DoppleTry2.VerifierNs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.BackTrackers
{
    public class BackTraceManager
    {
        private readonly BackTracer[] backTracers;
        private readonly Verifier[] verifiers;

        public BackTraceManager(List<InstructionNode> instructionNodes)
        {
            backTracers =
                           new BackTracer[]
                           {
                            new StackPopBackTracer(instructionNodes, this),
                            new LdArgBacktracer(instructionNodes,this),
                            new LdStaticFieldBackTracer(instructionNodes),
                            new LoadFieldByStackBackTracer(instructionNodes),
                            new LoadMemoryByOperandBackTracer(instructionNodes),
                            new TypedReferenceBackTracer(instructionNodes),
                            new ConditionionalsBackTracer(instructionNodes),
                            new CallBacktracer(instructionNodes, this)
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
    }
}
