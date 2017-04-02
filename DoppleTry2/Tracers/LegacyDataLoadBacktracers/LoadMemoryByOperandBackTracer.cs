using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using Dopple.InstructionNodes;

namespace Dopple.BackTracers
{
    class LindBacktracer : DynamicDataBacktracer
    {
        public override Code[] HandlesCodes => new[]
        {
            Code.Ldind_I1, Code.Ldind_U1, Code.Ldind_I2,
            Code.Ldind_U2, Code.Ldind_I4, Code.Ldind_U4,
            Code.Ldind_I8, Code.Ldind_I, Code.Ldind_R4,
            Code.Ldind_R8, Code.Ldind_Ref};

        protected override Predicate<InstructionNode> GetPredicate(InstructionNode instructionNode)
        {
            var addressArgs = instructionNode.DataFlowBackRelated.Where(x => x.ArgIndex == 0).SelectMany(x => x.Argument.GetDataOriginNodes());
            var specificCasesPredicates = new List<Predicate<InstructionNode>>();
            foreach (var addressArg in addressArgs)
            {
                Code nodeCode = addressArg.Instruction.OpCode.Code;
                Predicate<InstructionNode> specificCasePredicate;
                LdElemAddressNode addressArgAsLdelema = addressArg as LdElemAddressNode;
                if (addressArgAsLdelema != null)
                {
                    var arrayArgs = addressArgAsLdelema.ArrayBackArgs.SelectMany(y => y.GetDataOriginNodes()).ToArray();
                    var indexArgs = addressArgAsLdelema.IndexNodes.SelectMany(y => y.GetDataOriginNodes()).ToArray();
                    specificCasePredicate = x => x is StElemInstructionNode && LdElemBacktracer.ArrayAndIndexMatch((StElemInstructionNode) x, arrayArgs, indexArgs);
                    specificCasesPredicates.Add(specificCasePredicate);
                }
            }

            Predicate<InstructionNode> stindPredicate = x => x is StIndInstructionNode &&
                                        x.DataFlowBackRelated.Where(y => y.ArgIndex == 0)
                                        .SelectMany(y => y.Argument.GetDataOriginNodes())
                                        .SequenceEqual(x.DataFlowBackRelated.Where(y => y.ArgIndex == 0)
                                        .SelectMany(y => y.Argument.GetDataOriginNodes()));

            return x => specificCasesPredicates.Any(predicate => predicate(x)) || stindPredicate(x);

        }
    }
}
