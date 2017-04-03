using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dopple.BranchPropertiesNS;
using Dopple.InstructionNodes;

namespace Dopple.Tracers.PredciateProviders
{
    class StElemStateProvider : StoreDynamicDataStateProvider
    {
        internal StElemStateProvider(InstructionNode storeNode) : base(storeNode)
        {
            _IndexArgs = storeNode.DataFlowBackRelated.Where(x => x.ArgIndex == 1).SelectMany(x => x.Argument.GetDataOriginNodes()).ToList();
        }

        private List<InstructionNode> _IndexArgs;

        public override bool IsLoadNodeMatching(InstructionNode instructionNode)
        {
            LdElemInstructionNode loadNode = instructionNode as LdElemInstructionNode;
            if (loadNode == null)
            {
                return false;
            }
            var loadIndexArgs = loadNode.DataFlowBackRelated.Where(x => x.ArgIndex == 1).SelectMany(x => x.Argument.GetDataOriginNodes()).ToArray();
            if (!loadIndexArgs.Any(y => HaveEquivilentIndexNode(y, _IndexArgs)))
            {
                return false;
            }
            var loadArrayArgs = loadNode.DataFlowBackRelated.Where(x => x.ArgIndex == 1).SelectMany(x => x.Argument.GetDataOriginNodes()).ToArray();
            if (!loadIndexArgs.Intersect(ObjectNodes).Any())
            {
                return false;
            }
            return true;
        }

        public static bool ArrayAndIndexMatch(LdElemInstructionNode ldEelemToCheck, IEnumerable<InstructionNode> arrayArgs, IEnumerable<InstructionNode> indexArgs)
        {
            return ldEelemToCheck.DataFlowBackRelated.Where(y => y.ArgIndex == 0).SelectMany(y => y.Argument.GetDataOriginNodes()).SequenceEqual(arrayArgs) &&
                     
        }

        public static bool HaveEquivilentIndexNode(InstructionNode indexNodeToMatch, IEnumerable<InstructionNode> indexArgs)
        {
            if (indexNodeToMatch is LdImmediateInstNode)
            {
                int immediateValueToMatch = ((LdImmediateInstNode) indexNodeToMatch).ImmediateIntValue;
                return indexArgs.Any(x => x is LdImmediateInstNode && ((LdImmediateInstNode) x).ImmediateIntValue == immediateValueToMatch);
            }
            else
            {
                return indexArgs.Contains(indexNodeToMatch);
            }
        }

        public override bool ShareNonObjectArgs(StoreDynamicDataStateProvider storeStateProvider)
        {
            StElemStateProvider stElemStateProvider = storeStateProvider as StElemStateProvider;
            if (stElemStateProvider == null)
            { 
                return false;
            }
            if (!stElemStateProvider._IndexArgs.SequenceEqual(_IndexArgs))
            {
                return false;
            }
            return true;
        }

        internal override List<InstructionNode> GetObjectArgs(InstructionNode storeNode)
        {
            return StoreNode.DataFlowBackRelated.Where(x => x.ArgIndex == 0).SelectMany(x => x.Argument.GetDataOriginNodes()).ToList();
        }
    }
}
