using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using Dopple.InstructionNodes;

namespace Dopple.BackTrackers
{
    class LoadArrayElemBackTracer : SingeIndexBackTracer
    {
        private readonly Code[] _stArrayCodes = CodeGroups.StElemCodes.Concat(CodeGroups.LdArgCodes).ToArray();

        public LoadArrayElemBackTracer(List<InstructionNode> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override IEnumerable<InstructionNode> GetDataflowBackRelatedArgGroup(InstructionNode instWrapper)
        {
            return  _SingleIndexBackSearcher.SafeSearchBackwardsForDataflowInstrcutions(x => _stArrayCodes.Contains(x.Instruction.OpCode.Code)
                                                               && BackSearcher.HaveCommonStackPushAncestor(x, instWrapper), instWrapper);
        }

        public override Code[] HandlesCodes => new[] {Code.Ldelema,Code.Ldelem_I1,Code.Ldelem_U1,Code.Ldelem_I2
                                                     ,Code.Ldelem_U2 ,Code.Ldelem_I4 ,Code.Ldelem_U4 ,Code.Ldelem_I8
                                                     ,Code.Ldelem_I ,Code.Ldelem_R4 ,Code.Ldelem_R8 ,Code.Ldelem_Ref
                                                     ,Code.Ldelem_Any};
    }
}
