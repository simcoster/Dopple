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
        InstructionNodeFactory _InstructionNodeFactory = new InstructionNodeFactory();
        internal void ResolveVirtualMethods(List<InstructionNode> instructionNodes, out bool inlinlingWasMade)
        {
            inlinlingWasMade = false;

            foreach (VirtualCallInstructionNode virtualNodeCall in instructionNodes
                .Where(x => x is VirtualCallInstructionNode && ((VirtualCallInstructionNode)x).ResolveAttempted==false)
                .ToArray())
            {
                bool methodImplementationFound = false;
                var virtualMethodDeclaringTypeDefinition = virtualNodeCall.TargetMethod.DeclaringType.Resolve();
                var virtualMethodDeclaringTypeReference = virtualNodeCall.TargetMethod.DeclaringType;
                List<TypeDefinition> virtualMethodTypeInheritancePath = GetInheritancePath(virtualMethodDeclaringTypeReference);
                foreach (var objectArgument in virtualNodeCall.DataFlowBackRelated.Where(x => x.ArgIndex == 0).ToArray())
                {
                    foreach (var dataOriginNode in objectArgument.Argument.GetDataOriginNodes())
                    {
                        //TODO remove
                        TypeReference objectTypeReference = GetObjectType(dataOriginNode);
                        TypeDefinition objectTypeDefinition = objectTypeReference.Resolve();
                        if (objectTypeDefinition.IsAbstract)
                        {
                            virtualNodeCall.ResolveAttempted = true;
                            continue;
                        }
                        var methodImplementation = GetImplementation(virtualMethodDeclaringTypeDefinition, objectTypeDefinition, virtualNodeCall.TargetMethod.Resolve());
                        if (methodImplementation != null)
                        {
                            ReplaceVirtualCallWithImplementation(instructionNodes, virtualNodeCall, objectArgument.Argument, methodImplementation);
                            inlinlingWasMade = true;
                            methodImplementationFound = true;
                        }
                    }
                }
                if (!methodImplementationFound)
                {
                    virtualNodeCall.SelfRemove();
                    instructionNodes.Remove(virtualNodeCall);
                }
            }
        }

        private static void ReplaceVirtualCallWithImplementation(List<InstructionNode> instructionNodes, VirtualCallInstructionNode virtualNodeCall, InstructionNode objectArgument, MethodDefinition virtualMethodImpl)
        {
            var callOpCode = Instruction.Create(CodeGroups.AllOpcodes.First(x => x.Code == Code.Call), virtualMethodImpl);
            var callInstructionNode = new InlineableCallNode(callOpCode, virtualMethodImpl, virtualNodeCall.Method);
            virtualNodeCall.MergeInto(callInstructionNode,true);
            callInstructionNode.DataFlowBackRelated.RemoveAllTwoWay(x => x.ArgIndex == 0 && x.Argument != objectArgument);
            instructionNodes.Add(callInstructionNode);
            instructionNodes.Remove(virtualNodeCall);
        }

        private static MethodDefinition GetImplementation(TypeDefinition virtualMethodDeclaringTypeDefinition, TypeDefinition objectTypeDefinition
                                                ,MethodDefinition virtualMethodReference)
        {
            var objectTypeInheritancePath = GetInheritancePath(objectTypeDefinition).Select(x => x.Resolve()).ToArray();

            TypeDefinition implementingType;
            if (virtualMethodDeclaringTypeDefinition.IsInterface)
            {
                implementingType = objectTypeInheritancePath.FirstOrDefault(x => x.Interfaces.Any(y => y.Resolve().MetadataToken == virtualMethodDeclaringTypeDefinition.MetadataToken));
            }
            else
            {
                implementingType = objectTypeInheritancePath.FirstOrDefault(x => x.MetadataToken == virtualMethodDeclaringTypeDefinition.MetadataToken);
            }
            if (implementingType == null)
            {
                return null;
            }
            var sameNameMethods = implementingType.Methods.Where(x => x.Name == virtualMethodReference.Name);
            if (sameNameMethods.Count() != 1)
            {
                throw new Exception("None or more than 1 methods found that implement virtual");
            }
            return sameNameMethods.First();
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
            if (objectArg is RetInstructionNode)
            {
                return objectArg.Method.ReturnType;
            }
            throw new Exception("didn't find correct type");
        }
    }
}