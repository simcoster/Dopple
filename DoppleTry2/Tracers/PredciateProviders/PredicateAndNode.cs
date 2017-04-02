using Dopple.InstructionNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.Tracers.PredciateProviders
{
    class PredicateAndNode
    {
        public Predicate<InstructionNode> Predicate { get; set; }
        public InstructionNode StoreNode { get; set; }
    }
}
