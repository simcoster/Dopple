using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    class LoadArrayElemBackTracer : SingeIndexBackTracer
    {
        private readonly Code[] _stArrayCodes = new[]
            {
                Code.Stelem_Any, Code.Stelem_I, Code.Stelem_I2, 
                Code.Stelem_I1, Code.Stelem_I4, Code.Stelem_I8, Code.Stelem_R4, Code.Stelem_R8, Code.Stelem_Ref,
            }
        .Concat(LdArgBacktracer.LdArgCodes).ToArray();

        public LoadArrayElemBackTracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override IEnumerable<InstructionWrapper> GetDataflowBackRelatedArgGroup(InstructionWrapper instWrapper)
        {
            return SafeSearchBackwardsForDataflowInstrcutions(x => _stArrayCodes.Contains(x.Instruction.OpCode.Code)
                                                               && HaveCommonStackPushAncestor(x, instWrapper), instWrapper);
        }

        public override Code[] HandlesCodes => new[] {Code.Ldelema,Code.Ldelem_I1,Code.Ldelem_U1,Code.Ldelem_I2
                                                     ,Code.Ldelem_U2 ,Code.Ldelem_I4 ,Code.Ldelem_U4 ,Code.Ldelem_I8
                                                     ,Code.Ldelem_I ,Code.Ldelem_R4 ,Code.Ldelem_R8 ,Code.Ldelem_Ref
                                                     ,Code.Ldelem_Any};
    }
}
