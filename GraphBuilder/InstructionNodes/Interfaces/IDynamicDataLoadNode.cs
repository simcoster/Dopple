using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.InstructionNodes
{
    interface IDynamicDataLoadNode : IDataTransferingNode
    {
        bool AllPathsHaveAStoreNode { get; set; }
    }
}
