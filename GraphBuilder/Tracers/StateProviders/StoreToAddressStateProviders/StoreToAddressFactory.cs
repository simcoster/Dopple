using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.InstructionNodes;
using Mono.Cecil.Cil;
using Dopple.Tracers.StateProviders.StoreToAddressStateProviders;

namespace Dopple.Tracers.StateProviders
{
    class StoreToAddressFactory
    {
        public static IEnumerable<StoreDynamicDataStateProvider> GetStoreToAddressStateProvider(InstructionNode storeInderectNode)
        {
            var AddressNodes = storeInderectNode.DataFlowBackRelated.Where(x => x.ArgIndex == 0).SelectMany(x => x.Argument.GetDataOriginNodes());
            var groupContainingFirstArg = LoadAddressCodeGroups.First(x => x.Contains(AddressNodes.First().Instruction.OpCode.Code));
            if (!AddressNodes.Skip(1).All(x => groupContainingFirstArg.Contains(x.Instruction.OpCode.Code)))
            {
                throw new Exception("Mixed type store to address node");
            }
            if (groupContainingFirstArg == LoadLocAddress)
            {
                return AddressNodes.Select(x => new StoreToLocationByAddress(storeInderectNode, x));
            }
            if (groupContainingFirstArg == LoadEelemAddress)
            {
                return AddressNodes.Select(x => new StoreElementByAddressStateProvider(storeInderectNode, x));
            }
            if (groupContainingFirstArg == LoadArgAddress)
            {
                return AddressNodes.Select(x => new StoreArgumentByAddressStateProvider(storeInderectNode, x));
            }
            if (groupContainingFirstArg == LoadFieldAddress)
            {
                return AddressNodes.Select(x => new StoreFieldByAddressStateProvider(storeInderectNode, x));
            }
            if (groupContainingFirstArg == LoadStaticFieldAddress)
            {
                return AddressNodes.Select(x => new StoreStaticFieldByAddressStateProvider(storeInderectNode, x));
            }
            throw new Exception("Couldnt detect store address type");
        }

        private static Code[] LoadLocAddress = new[]{ Code.Ldloca, Code.Ldloca_S }.Concat(CodeGroups.LdLocCodes).ToArray();
        private static Code[] LoadEelemAddress = new[] { Code.Ldelema }.Concat(CodeGroups.LdElemCodes).ToArray();
        private static Code[] LoadArgAddress = new[]{ Code.Ldarga, Code.Ldarga_S }.Concat(CodeGroups.LdArgCodes).ToArray();
        private static Code[] LoadFieldAddress = new[]{ Code.Ldflda }.Concat(CodeGroups.LoadFieldCodes).ToArray();
        private static Code[] LoadStaticFieldAddress = new[] { Code.Ldsflda, Code.Ldsfld };

        private static Code[][] LoadAddressCodeGroups = { LoadLocAddress, LoadEelemAddress, LoadArgAddress, LoadFieldAddress, LoadStaticFieldAddress };
    }
}
