using DoppleTry2.InstructionWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.VerifierNs
{
    class TwoWayVerifier : Verifier
    {
        public TwoWayVerifier(List<InstructionWrapper> instructionWrappers) : base(instructionWrappers)
        {

        }

        public override void Verify(InstructionWrapper instructionWrapper)
        {
            //throw new NotImplementedException();
        }

        public void Verify(IEnumerable<InstructionWrapper> instructionWrappers)
        {
            foreach (var inst in instructionWrappers.OrderByDescending(x => x.InstructionIndex))
            {
                foreach (var backInst in inst.BackDataFlowRelated.ArgumentList)
                {
                    if (!backInst.Argument.ForwardDataFlowRelated.ArgumentList.Select(x => x.Argument).Contains(inst))
                    {
                        throw new Exception();
                    }
                }
                foreach (var forInst in inst.ForwardDataFlowRelated.ArgumentList)
                {
                    if (!forInst.Argument.BackDataFlowRelated.ArgumentList.Select(x => x.Argument).Contains(inst))
                    {
                        throw new Exception();
                    }
                }
            }
        }
    }
}
