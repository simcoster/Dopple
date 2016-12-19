using DoppleTry2.InstructionWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSimilarity.EditOperations
{
    internal class NodeSubstitution : NodeEditOperation
    {
        public InstructionWrapper ReplacedWith;

        public NodeSubstitution(List<InstructionWrapper> graph) : base(graph)
        {
        }

        public override int Cost
        {
            get
            {
                if (InstructionWrapper.Instruction.OpCode.Code == ReplacedWith.Instruction.OpCode.Code)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }

            }
        }

        public override string Name
        {
            get
            {
                return "NodeSubstitution";
            }
        }

        protected override List<EdgeEditOperation> GetEdgeOperations()
        {
            throw new NotImplementedException();
        }
    }
}
