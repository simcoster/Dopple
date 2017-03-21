using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.InstructionNodes
{
    interface IObjectOrAddressRequiringNode
    {
        int ObjectOrAddressArgIndex { get; }
        bool ObjectOrAddressArgsResolved { get; set; }
    }
}
