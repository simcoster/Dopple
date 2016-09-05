using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    class LdArgBacktracer : BackTracer
    {
        public LdArgBacktracer(List<InstructionWrapper> instructionsWrappers) : base(instructionsWrappers)
        {
        }

        protected override int[] GetDataflowBackRelatedIndices(int instructionIndex, Node currentNode)
        {
            var instructionWrapper = InstructionsWrappers[instructionIndex];
            int ldArgLoc;
            switch (instructionWrapper.Instruction.OpCode.Code)
            {
                case Code.Ldarg_0:
                    ldArgLoc = 0;
                    break;
                case Code.Ldarg_1:
                    ldArgLoc = 1;
                    break;
                case Code.Ldarg_2:
                    ldArgLoc = 2;
                    break;
                case Code.Ldarg_3:
                    ldArgLoc = 3;
                    break;
                default:
                    ldArgLoc = Convert.ToInt32(instructionWrapper.Instruction.Operand);
                    break;
            }
            Code[] relevantCodes = {Code.Starg, Code.Starg_S};
            var stArgIndex = SafeSearchBackwardsForDataflowInst(
                x => relevantCodes.Contains(x.Instruction.OpCode.Code) && Convert.ToInt32(x.Instruction.Operand) == ldArgLoc ,
                instructionIndex);
            if (stArgIndex == null)
            {
                currentNode.HasBackNodes = false;
                return new int[0];
            }
            else
            {
                return new[] {stArgIndex.Value};
            }
        }

        public override Code[] HandlesCodes => new []
        {
            Code.Ldarg,Code.Ldarg_0, Code.Ldarg_1, Code.Ldarg_2, Code.Ldarg_3, Code.Ldarg_S, 
            Code.Ldarga, Code.Ldarga_S
        };
    }
}
