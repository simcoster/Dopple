using System;
using System.Collections.Generic;
using Dopple.InstructionNodes;
using System.Linq;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace Dopple
{
    internal class VirtualMethodResolver
    {
        internal static void ResolveVirtualMethods(List<InstructionNode> instructionNodes, out bool inlinlingWasMade)
        {
            foreach (NonInlineableCallInstructionNode virtualNodeCall in instructionNodes.Where(x => x.Instruction.OpCode.Code == Code.Callvirt))
            {
                //.SelectMany(x => 
                foreach (var objectArg in virtualNodeCall.DataFlowBackRelated.Where(x => x.ArgIndex == 0).SelectMany(x => x.Argument.GetDataOriginNodes()))
                {
                    var objectMethods = GetObjectMethods(objectArg); 
                }
            }
            inlinlingWasMade = false;
        }

        private static ICollection<MethodDefinition> GetObjectMethods(InstructionNode objectArg)
        {
            if (objectArg is NewObjectNode)
            {
                return ((MethodReference) objectArg.Instruction.Operand).DeclaringType.Resolve().Methods;
            }
            if (objectArg is LdArgInstructionNode)
            {
                return ((LdArgInstructionNode) objectArg).ArgType.Resolve().Methods;
            }
            return new MethodDefinition[0];
            //throw new Exception("No object methods found");

        }

        private static bool IsProvidingObject(InstructionNode argument)
        {
            if (argument is NewObjectNode)
            {
                return true;
            }
            return false;
        }
    }
}