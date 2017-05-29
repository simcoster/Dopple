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
        internal void ResolveVirtualMethods(List<InstructionNode> instructionNodes, out bool inlinlingWasMade, out bool dynamicInMethods)
        {
            inlinlingWasMade = false;
            dynamicInMethods = false;

            foreach (VirtualCallInstructionNode virtualNodeCall in instructionNodes
                .Where(x => x is VirtualCallInstructionNode)
                .ToArray())
            {
                var virtualMethodDeclaringTypeDefinition = virtualNodeCall.TargetMethod.DeclaringType.Resolve();
                var virtualMethodDeclaringTypeReference = virtualNodeCall.TargetMethod.DeclaringType;
                List<TypeDefinition> virtualMethodTypeInheritancePath = GetInheritancePath(virtualMethodDeclaringTypeReference);
                var objectArgs = virtualNodeCall.DataFlowBackRelated.Where(x => x.ArgIndex == 0).ToArray();
                foreach (var objectArgument in objectArgs)
                {
                    var dataOriginNodes = objectArgument.Argument.GetDataOriginNodes();
                    foreach(var originNodeNoLongerFound in virtualNodeCall.DataOriginNodeIsResolveable.Keys.Except(dataOriginNodes.ToList()).ToList())
                    {
                        virtualNodeCall.DataOriginNodeIsResolveable.Remove(originNodeNoLongerFound);
                    }
                    foreach (var dataOriginNode in dataOriginNodes.Except(virtualNodeCall.DataOriginNodeIsResolveable.Keys))
                    {
                        if (dataOriginNode is InlineableCallNode || dataOriginNode is VirtualCallInstructionNode)
                        {
                            virtualNodeCall.DataOriginNodeIsResolveable.Add(dataOriginNode, false);
                            continue;
                        }
                        if (dataOriginNode.Instruction.OpCode.Code == Code.Ldnull)
                        {
                            virtualNodeCall.DataOriginNodeIsResolveable.Add(dataOriginNode, false);
                            continue;
                        }
                        MethodDefinition methodImplementation;
                        if (virtualNodeCall.TargetMethod.Name == "Invoke")
                        {
                            bool allArgsResolved = true;
                            if (dataOriginNode is NewObjectNode)
                            {
                                var loadFunctionArgs = dataOriginNode.DataFlowBackRelated.Where(x => x.ArgIndex == 1);
                                foreach(var loadFtnArg in loadFunctionArgs)
                                {
                                    if (loadFtnArg.Argument is LoadFunctionNode)
                                    {
                                        MethodDefinition loadedFunc = ((LoadFunctionNode) loadFtnArg.Argument).LoadedFunction;
                                        AddVirtualCallImplementation(instructionNodes, virtualNodeCall, dataOriginNode, loadedFunc, out dynamicInMethods);
                                    }
                                    else
                                    {
                                        allArgsResolved = false;
                                    }
                                }
                            }
                            else
                            {
                                allArgsResolved = false;
                            }
                            virtualNodeCall.DataOriginNodeIsResolveable.Add(dataOriginNode, allArgsResolved);
                            continue;
                        }
                        TypeReference objectTypeReference;
                        bool typeFound = TryGetObjectType(dataOriginNode, out objectTypeReference);
                        if (!typeFound)
                        {
                            Console.WriteLine("couldnt detect type of node " + dataOriginNode.Instruction +" offset:" + dataOriginNode.Instruction.Offset);
                            continue;
                            //throw new Exception("Couldn't detect type of node" + dataOriginNode.Instruction);
                        }
                        TypeDefinition objectTypeDefinition = null;
                        if (objectTypeReference.IsArray)
                        {
                            objectTypeDefinition = ArrayTypeDefinition;
                        }
                        else
                        {
                            objectTypeDefinition = objectTypeReference.Resolve();
                        }
                        if (objectTypeDefinition == ArrayTypeDefinition)
                        { 
                            //TODO redesign
                            //this is a special case, I want to be able to inline the native command of GetValue
                            if (virtualNodeCall.TargetMethod.FullName == "System.Object System.Array::GetValue(System.Int32)")
                            {
                                AddLoadElemImplementation(instructionNodes, virtualNodeCall, objectArgument);
                                virtualNodeCall.DataOriginNodeIsResolveable.Add(dataOriginNode, true);
                                continue;
                            }
                        }
                        else
                        {
                            if (objectTypeDefinition.IsAbstract)
                            {
                                virtualNodeCall.DataOriginNodeIsResolveable.Add(dataOriginNode, false);
                                inlinlingWasMade = true;
                                continue;
                            }
                        }
                        methodImplementation = GetImplementation(virtualMethodDeclaringTypeDefinition, objectTypeDefinition, virtualNodeCall.TargetMethod.Resolve());
                        if (methodImplementation != null)
                        {
                            AddVirtualCallImplementation(instructionNodes, virtualNodeCall, objectArgument.Argument, methodImplementation, out dynamicInMethods);
                            virtualNodeCall.DataOriginNodeIsResolveable.Add(dataOriginNode, true);
                            inlinlingWasMade = true;
                        }
                        else
                        {
                            virtualNodeCall.DataOriginNodeIsResolveable.Add(dataOriginNode, true);
                        }
                    }
                }
                if (virtualNodeCall.DataOriginNodeIsResolveable.All(x => x.Value == true))
                {
                    virtualNodeCall.SelfRemove();
                    instructionNodes.Remove(virtualNodeCall);
                }
            }
        }

        private void AddLoadElemImplementation(List<InstructionNode> instructionNodes, VirtualCallInstructionNode virtualNodeCall, IndexedArgument objectArgument)
        {
            var callOpCode = Instruction.Create(CodeGroups.AllOpcodes.First(x => x.Code == Code.Ldelem_Ref));
            var loadElementNode = new LdElemInstructionNode(callOpCode, virtualNodeCall.Method);
            AddImplementation(instructionNodes, virtualNodeCall, loadElementNode);
        }
   
        private void AddVirtualCallImplementation(List<InstructionNode> instructionNodes, VirtualCallInstructionNode virtualNodeCall, InstructionNode objectArgument, MethodDefinition virtualMethodImpl, out bool methodStoresDynamic)
        {
            methodStoresDynamic = false;
            var callOpCode = Instruction.Create(CodeGroups.AllOpcodes.First(x => x.Code == Code.Call), virtualMethodImpl);
            CallNode virtualImplementationNode;
            if (!virtualMethodImpl.HasBody)
            {
                virtualImplementationNode = new NonInlineableCallInstructionNode(callOpCode, virtualNodeCall.Method);
            }
            else
            {
                virtualImplementationNode = new InlineableCallNode(callOpCode, virtualMethodImpl, virtualNodeCall.Method);
                methodStoresDynamic = IsStoringDynamicFromOutside(virtualNodeCall, virtualMethodImpl);
            }
            AddImplementation(instructionNodes, virtualNodeCall, virtualImplementationNode);
            virtualImplementationNode.OriginalVirtualNode = virtualNodeCall;
            virtualImplementationNode.DataFlowBackRelated.RemoveAllTwoWay(x => x.ArgIndex == 0 && x.Argument != objectArgument);

        }
        private void AddImplementation(List<InstructionNode> instructionNodes, VirtualCallInstructionNode virtualNodeCall, InstructionNode virtualImplementationNode)
        {
            AddPseudoSplitNode(virtualNodeCall, instructionNodes);

            virtualNodeCall.MergeInto(virtualImplementationNode, true);
            virtualImplementationNode.ProgramFlowResolveDone = true;
            virtualImplementationNode.StackBacktraceDone = true;
            virtualImplementationNode.InliningProperties = virtualNodeCall.InliningProperties;

            virtualImplementationNode.ProgramFlowBackRoutes.RemoveAllTwoWay();
            virtualImplementationNode.ProgramFlowBackRoutes.AddTwoWay(virtualNodeCall.PseudoSplitNode);

            instructionNodes.Insert(instructionNodes.IndexOf(virtualNodeCall), virtualImplementationNode);
        }

        private void AddPseudoSplitNode(VirtualCallInstructionNode virtualNodeCall, List<InstructionNode> instructionNodes)
        {
            if (virtualNodeCall.PseudoSplitNode == null)
            {
                virtualNodeCall.PseudoSplitNode = new PseudoSplitNode(virtualNodeCall.Method);
                virtualNodeCall.PseudoSplitNode.ProgramFlowBackRoutes.AddTwoWay(virtualNodeCall.ProgramFlowBackRoutes);
                virtualNodeCall.ProgramFlowBackRoutes.RemoveAllTwoWay();
                virtualNodeCall.PseudoSplitNode.ProgramFlowForwardRoutes.AddTwoWay(virtualNodeCall);
                instructionNodes.Insert(instructionNodes.IndexOf(virtualNodeCall), virtualNodeCall.PseudoSplitNode);
            }
        }

        private bool IsStoringDynamicFromOutside(VirtualCallInstructionNode virtualNodeCall, MethodDefinition virtualMethodImpl)
        {
            Code[] storeDynamicCodes = CodeGroups.StElemCodes.Concat(CodeGroups.StoreFieldCodes).ToArray();
            if (virtualMethodImpl.Body.Instructions.All(x => !storeDynamicCodes.Contains(x.OpCode.Code)))
            {
                return false;
            }
            if (!virtualNodeCall.TargetMethod.HasThis && virtualNodeCall.TargetMethod.Parameters.All(x => x.ParameterType.IsValueType))
            {
                return false;
            }
            return true;
        }

        private MethodDefinition GetImplementation(TypeDefinition virtualMethodDeclaringTypeDefinition, TypeDefinition objectTypeDefinition
                                                ,MethodDefinition virtualMethodReference)
        {
            var objectTypeInheritancePath = GetInheritancePath(objectTypeDefinition).Select(x => x.Resolve()).ToArray();

            IEnumerable<MethodDefinition> methodImpls = null;
            foreach (var typeDef in objectTypeInheritancePath)
            {
                methodImpls = typeDef.Methods.Where(x => virtualMethodReference.Name == x.Name && !x.IsAbstract)
                                             .Where(x => HasCorrespondingParams(x, virtualMethodReference));


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

        private static bool HasCorrespondingParams(MethodDefinition firstMethod, MethodDefinition secondMethod)
        {
            var secondMethodParams = secondMethod.Parameters.Where(x => !x.ParameterType.IsGenericParameter).ToList();
            foreach (var firstMethodParam in firstMethod.Parameters.Where(x => !x.ParameterType.IsGenericParameter))
            {
                var matchingSecondParam = secondMethodParams.FirstOrDefault(x => x.Name == firstMethodParam.Name && x.ParameterType == firstMethodParam.ParameterType);
                if (matchingSecondParam == null)
                {
                    return false;
                }
                secondMethodParams.Remove(matchingSecondParam);
            }
            return true;
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
            if (objectArg is NewObjectNode || objectArg is NewObjectNodeWithConstructor)
            {
                foundType = ((MethodReference) objectArg.Instruction.Operand).DeclaringType;
            }
            if (objectArg is RetInstructionNode)
            {
                foundType = objectArg.Method.ReturnType;
            }
            if (objectArg is LoadFieldNode)
            {
                foundType = ((LoadFieldNode)objectArg).FieldDefinition.FieldType;
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