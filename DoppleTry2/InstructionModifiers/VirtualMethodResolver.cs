using System;
using System.Collections.Generic;
using Dopple.InstructionNodes;
using System.Linq;
using Mono.Cecil.Cil;
using Mono.Cecil;
using System.Text.RegularExpressions;

namespace Dopple
{
    internal class VirtualMethodResolver
    {
        TypeDefinition ArrayTypeDefinition = ModuleDefinition.ReadModule(typeof(object).Module.FullyQualifiedName).Types.First(x => x.FullName == "System.Array");
        InstructionNodeFactory _InstructionNodeFactory = new InstructionNodeFactory();
        internal void ResolveVirtualMethods(List<InstructionNode> instructionNodes, out bool inlinlingWasMade)
        {
            inlinlingWasMade = false;

            foreach (VirtualCallInstructionNode virtualNodeCall in instructionNodes
                .Where(x => x is VirtualCallInstructionNode && ((VirtualCallInstructionNode)x).ResolveAttempted==false)
                .ToArray())
            {
                var virtualMethodDeclaringTypeDefinition = virtualNodeCall.TargetMethod.DeclaringType.Resolve();
                var virtualMethodDeclaringTypeReference = virtualNodeCall.TargetMethod.DeclaringType;
                List<TypeDefinition> virtualMethodTypeInheritancePath = GetInheritancePath(virtualMethodDeclaringTypeReference);
                var objectArgs = virtualNodeCall.DataFlowBackRelated.Where(x => x.ArgIndex == 0).ToArray();
                bool unresolvedDataArgsExist = false;
                foreach (var objectArgument in objectArgs)
                {
                    foreach (var dataOriginNode in objectArgument.Argument.GetDataOriginNodes().Except(virtualNodeCall.ResolvedObjectArgs))
                    {
                        unresolvedDataArgsExist = true;
                        virtualNodeCall.ResolvedObjectArgs.Add(dataOriginNode);
                        if (dataOriginNode is InlineableCallNode || dataOriginNode is VirtualCallInstructionNode)
                        {
                            //Wait for next round
                            continue;
                        }
                        TypeReference objectTypeReference;
                        bool typeFound = TryGetObjectType(dataOriginNode, out objectTypeReference);
                        if (!typeFound)
                        {
                            continue;
                        }
                        TypeDefinition objectTypeDefinition;
                        if (objectTypeReference.IsArray)
                        {
                            objectTypeDefinition = ArrayTypeDefinition;
                        }
                        else
                        {
                            objectTypeDefinition = objectTypeReference.Resolve();
                            if (objectTypeDefinition.IsAbstract)
                            {
                                continue;
                            }
                        }
                        var methodImplementation = GetImplementation(virtualMethodDeclaringTypeDefinition, objectTypeDefinition, virtualNodeCall.TargetMethod.Resolve());
                        if (methodImplementation != null)
                        {
                            AddVirtualCallImplementation(instructionNodes, virtualNodeCall, objectArgument.Argument, methodImplementation);
                            inlinlingWasMade = true;
                            virtualNodeCall.DataFlowBackRelated.RemoveTwoWay(objectArgument);
                        }
                        else
                        {
                            virtualNodeCall.DataFlowBackRelated.RemoveTwoWay(objectArgument);
                        }
                    }
                }
                if (unresolvedDataArgsExist == false)
                {
                    virtualNodeCall.SelfRemove();
                    instructionNodes.Remove(virtualNodeCall);
                }
            }
        }

        private static void AddVirtualCallImplementation(List<InstructionNode> instructionNodes, VirtualCallInstructionNode virtualNodeCall, InstructionNode objectArgument, MethodDefinition virtualMethodImpl)
        {
            var callOpCode = Instruction.Create(CodeGroups.AllOpcodes.First(x => x.Code == Code.Call), virtualMethodImpl);
            var callInstructionNode = new InlineableCallNode(callOpCode, virtualMethodImpl, virtualNodeCall.Method);
            virtualNodeCall.MergeInto(callInstructionNode,true);
            callInstructionNode.DataFlowBackRelated.RemoveAllTwoWay(x => x.ArgIndex == 0 && x.Argument != objectArgument);
            callInstructionNode.InliningProperties = virtualNodeCall.InliningProperties;
            instructionNodes.Insert(instructionNodes.IndexOf(virtualNodeCall), callInstructionNode);
        }

        private static MethodDefinition GetImplementation(TypeDefinition virtualMethodDeclaringTypeDefinition, TypeDefinition objectTypeDefinition
                                                ,MethodDefinition virtualMethodReference)
        {
            var objectTypeInheritancePath = GetInheritancePath(objectTypeDefinition).Select(x => x.Resolve()).ToArray();

            IEnumerable<MethodDefinition> methodImpls = null;
            foreach(var typeDef in objectTypeInheritancePath)
            {
                if (typeDef.FullName == "System.Array")
                {
                    //Arrays are special
                    methodImpls = typeDef.Methods.Where(x => virtualMethodReference.Name == x.Name && !x.IsAbstract);
                }
                else
                {
                    //methodImpl = typeDef.Methods.FirstOrDefault(x => MethodSignitureMatch(virtualMethodReference.FullName, x.FullName) && !x.IsAbstract);
                    methodImpls = typeDef.Methods.Where(x => MethodSignitureMatch(virtualMethodReference.Name, x.Name) && !x.IsAbstract);
                }
                if (methodImpls.Count() > 1)
                {
                    throw new Exception("Too many implmenetations");
                }

                if (methodImpls.Count() ==1)
                {
                    break;
                }
            }
            return methodImpls.FirstOrDefault();
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

        private static bool TryGetObjectType(InstructionNode objectArg, out TypeReference foundType)
        {
            foundType = null;
            if (objectArg is NonInlineableCallInstructionNode)
            {
                foundType = ((CallNode) objectArg).TargetMethod.ReturnType;
            }
            if (objectArg is LdArgInstructionNode)
            {
                foundType = ((LdArgInstructionNode) objectArg).ArgType;
            }
            if (objectArg is NewObjectNode || objectArg is ConstructorNewObjectNode)
            {
                foundType = ((MethodReference) objectArg.Instruction.Operand).DeclaringType;
            }
            if (objectArg is RetInstructionNode)
            {
                foundType = objectArg.Method.ReturnType;
            }
            if (foundType == null)
            {
                return false;
            }
            return true ;
            //TODO remove
            //throw new Exception("didn't find correct type");
        }

        private static bool MethodSignitureMatch(string method1, string method2)
        {
            string[] methodNames = new string[] { method1, method2 };
            string pattern = " .*::";
            string replacement = " ";
            Regex rgx = new Regex(pattern);
            for(int i=0; i<methodNames.Length; i++)
            {
                methodNames[i] = rgx.Replace(methodNames[i], replacement);
                methodNames[i] = methodNames[i].Replace("TSource", "T");
            }
            
            return methodNames[0] == methodNames[1];
        }
    }
}