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
                List<TypeDefinition> virtualMethodTypeInheritancePath = GetInheritancePath(virtualMethodDeclaringTypeReference);

                foreach (var objectArg in virtualNodeCall.DataFlowBackRelated.Where(x => x.ArgIndex == 0).SelectMany(x => x.Argument.GetDataOriginNodes()))
                //foreach (var objectArg in virtualNodeCall.DataFlowBackRelated.Where(x => x.ArgIndex == 0).Select(x => x.Argument))
                {
                    TypeReference objectTypeReference = GetObjectType(objectArg);
                    TypeDefinition objectTypeDefinition = objectTypeReference.Resolve();

                    if (IsImplementing(virtualMethodDeclaringTypeDefinition, virtualMethodTypeInheritancePath, objectTypeDefinition))
                    {
                        var virtualMethodImpl = objectTypeDefinition.Methods.First(x => x.MetadataToken == virtualNodeCall.TargetMethod.Resolve().MetadataToken);
                    }
                    var objectMethods = GetObjectMethods(objectArg);
                }
            }
            inlinlingWasMade = false;
        }

        private static bool IsImplementing(TypeDefinition virtualMethodDeclaringTypeDefinition, List<TypeDefinition> virtualMethodTypeInheritancePath, TypeDefinition objectTypeDefinition)
        {
            var objectTypeInheritancePath = GetInheritancePath(objectTypeDefinition).Select(x => x.Resolve());

            if (virtualMethodDeclaringTypeDefinition.IsInterface)
            {
                if (!GetAllInterfaces(objectTypeInheritancePath).Any(x => x.MetadataToken == virtualMethodDeclaringTypeDefinition.MetadataToken))
                {
                    return false;
                }
            }
            else if (!virtualMethodTypeInheritancePath.Any(x => x.MetadataToken == objectTypeDefinition.MetadataToken))
            {
                return false;
            }
            return true;
        }

        private static IEnumerable<TypeReference> GetAllInterfaces(IEnumerable<TypeDefinition> inheritancePath)
        {
            return inheritancePath.SelectMany(x => x.Interfaces.SelectMany(y => GetInheritancePath(y))).Distinct();
        }

        private static List<TypeDefinition> GetInheritancePath(TypeReference baseTypeReference)
        {
            List<TypeDefinition> typeInheritancePath = new List<TypeDefinition>();
            TypeReference currTypeRef = baseTypeReference;
            while (currTypeRef != null)
            {
                TypeDefinition currTypeDef = currTypeRef.Resolve();
                typeInheritancePath.Add(currTypeDef);
                currTypeRef = currTypeDef.BaseType;
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