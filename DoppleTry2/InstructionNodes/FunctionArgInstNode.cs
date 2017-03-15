using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Dopple.InstructionNodes
{
    [DataContract]
    public abstract class FunctionArgInstNode : InstructionNode, IDataTransferingNode
    {
        public FunctionArgInstNode(Instruction instruction, MethodDefinition method) : base(instruction, method)
        {
            ArgIndex = GetArgIndex(instruction, method);
            string argNameTemp;
            TypeReference argTypeTemp;
            ParamDefinition = GetArgNameAndType(ArgIndex, method, out argNameTemp, out argTypeTemp);
            ArgName = argNameTemp;
            ArgType = argTypeTemp;
        }

        private static ParameterDefinition GetArgNameAndType(int argIndex, MethodDefinition method, out string argName, out TypeReference argType)
        {
            int parameterArrayIndex;
            if (method.IsStatic)
            {
                parameterArrayIndex = argIndex;
            }
            else
            {
                parameterArrayIndex = argIndex - 1;
                if (parameterArrayIndex == -1)
                {
                    argName = "this";
                    argType = method.DeclaringType;
                    return null;
                }
            }
            argName = method.Parameters[parameterArrayIndex].Name;
            argType = method.Parameters[parameterArrayIndex].ParameterType;
            return method.Parameters[parameterArrayIndex];
        }

        public int ArgIndex { get; set; }
        public TypeReference ArgType { get; set; }
        public string ArgName { get; set; }
        public ParameterDefinition ParamDefinition { get; private set; }
        public abstract int DataFlowDataProdivderIndex { get; }

        private int GetArgIndex(Instruction instruction, MethodDefinition method)
        {
            int indexByCode = 0;
            if (TryGetCodeArgIndex(instruction, out indexByCode) != false)
            {
                return indexByCode;
            }
            int indexByOperand;
            if (instruction.Operand is ValueType)
                indexByOperand = Convert.ToInt32(instruction.Operand);
            else if (instruction.Operand is VariableDefinition)
                indexByOperand = ((VariableDefinition) instruction.Operand).Index;
            else if (instruction.Operand is ParameterDefinition)
                indexByOperand = ((ParameterDefinition) instruction.Operand).Index;
            else
            {
                throw new Exception("shouldn't get here");
            }
            if (method.IsStatic)
            {
                return indexByOperand;
            }
            else
            {
                return indexByOperand + 1;
            }
        }

        protected virtual bool TryGetCodeArgIndex(Instruction instruction, out int index)
        {
            index = -1;
            return false;
        }
    }
}
