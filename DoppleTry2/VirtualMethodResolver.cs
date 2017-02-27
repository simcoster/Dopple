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
                var virtualMethodDeclaringTypeDefinition = virtualNodeCall.TargetMethod.DeclaringType.Resolve();
                var virtualMethodDeclaringTypeReference = virtualNodeCall.TargetMethod.DeclaringType;
                List<TypeReference> virtualMethodTypeInheritancePath = GetInheritancePath(virtualMethodDeclaringTypeReference);

                foreach (var objectArg in virtualNodeCall.DataFlowBackRelated.Where(x => x.ArgIndex == 0).SelectMany(x => x.Argument.GetDataOriginNodes()))
                //foreach (var objectArg in virtualNodeCall.DataFlowBackRelated.Where(x => x.ArgIndex == 0).Select(x => x.Argument))

                {
                    TypeReference objectTypeReference = GetObjectType(objectArg);
                    TypeDefinition objectTypeDefinition = objectTypeReference.Resolve();
                    if (virtualMethodDeclaringTypeDefinition.IsInterface)
                    {
                        if (!objectTypeDefinition.Interfaces.Contains(virtualMethodDeclaringTypeReference))
                        {
                            continue;
                        }
                    }
                    else if (!virtualMethodTypeInheritancePath.Contains(objectTypeReference))
                    {
                        continue;
                    }
                    else
                    {

                    }

                    var objectMethods = GetObjectMethods(objectArg);
                }
            }
            inlinlingWasMade = false;
        }

        private static List<TypeReference> GetInheritancePath(TypeReference baseTypeReference)
        {
            TypeReference currTypeRef = baseTypeReference;
            List<TypeReference> typeInheritancePath = new List<TypeReference>();
            while (currTypeRef != null)
            {
                typeInheritancePath.Add(currTypeRef);
                currTypeRef = currTypeRef.DeclaringType;
            }
            return typeInheritancePath;
        }

        private static TypeReference GetObjectType(InstructionNode objectArg)
        {
            if (objectArg is CallNode)
            {
                return ((CallNode) objectArg).TargetMethod.ReturnType;
            }
            if (objectArg is LdArgInstructionNode)
            {
                return ((LdArgInstructionNode) objectArg).ArgType;
            }
            if (objectArg is NewObjectNode || objectArg is ConstructorNewObjectNode)
            {
                return ((MethodReference) objectArg.Instruction.Operand).DeclaringType;
            }
            return null;
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