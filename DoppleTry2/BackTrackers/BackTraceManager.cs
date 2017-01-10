using DoppleTry2.BackTrackers;
using DoppleTry2.InstructionNodes;
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
        }

        public void BackTrace(InstructionNode instructionNode)
        {
            backTracers.Where(x => x.HandlesCodes.Contains(instructionNode.Instruction.OpCode.Code))
                        .ForEach(x => x.AddBackDataflowConnections(instructionNode));
        }
        public IEnumerable<BackTracer> GetRelevantBackTracers(InstructionNode instructionNode)
        {
            return backTracers.Where(x => x.HandlesCodes.Contains(instructionNode.Instruction.OpCode.Code));
        }
    }
}
