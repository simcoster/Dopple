using System.Collections.Generic;
using System.Linq;
using DoppleTry2.InstructionModifiers;
using Mono.Cecil.Cil;
using Mono.Cecil;
using System;

namespace DoppleTry2
{
    public class InstructionWrapper
    {
        private ArgList _BackDataFlowRelated = new ArgList();

        public InstructionWrapper(Instruction instruction, MethodDefinition method)
        {
            Instruction = instruction;
            Method = method;
            StackPushCount = GetStackPushCount(instruction);
            StackPopCount = GetStackPopCount(instruction);
            MemoryReadCount = MemoryProperties.GetMemReadCount(instruction.OpCode.Code);
            MemoryStoreCount = MemoryProperties.GetMemStoreCount(instruction.OpCode.Code);
            LocIndex = LdStLocProperties.GetLocIndex(instruction);
            ArgIndex = GetArgIndex(instruction);
            ImmediateIntValue = GetImmediateInt(instruction);
        }

        private int GetArgIndex(Instruction instruction)
        {
            Code[] argCodes = { Code.Ldarg, Code.Ldarg_0, Code.Ldarg_1, Code.Ldarg_2, Code.Ldarg_3, Code.Ldarg_S,
                                Code.Ldarga, Code.Ldarga_S,
                                Code.Starg, Code.Starg_S};

            if (!argCodes.Contains(instruction.OpCode.Code))
            {
                return -1;
            }
            switch (instruction.OpCode.Code)
            {
                case Code.Ldarg_0:
                    return 0;
                case Code.Ldarg_1:
                    return 1;
                case Code.Ldarg_2:
                    return 2;
                case Code.Ldarg_3:
                    return 3;
            }

            if (instruction.Operand is ValueType)
            {
                return Convert.ToInt32(instruction.Operand);
            }
            else
            {
                if (instruction.Operand is VariableDefinition)
                {
                    return ((VariableDefinition)instruction.Operand).Index;
                }
                else if (instruction.Operand is ParameterDefinition)
                {
                    return ((ParameterDefinition)instruction.Operand).Index;
                }
            }
            throw new Exception("shouldn't get here");
        }

        private int? GetImmediateInt(Instruction instruction)
        {
            var imeddiateFixedValue = new[]
            {
                Code.Ldc_I4_0, Code.Ldc_I4_1, Code.Ldc_I4_2, Code.Ldc_I4_3, Code.Ldc_I4_4, Code.Ldc_I4_5,
                Code.Ldc_I4_6, Code.Ldc_I4_7, Code.Ldc_I4_8
            };

            var imeddiateOperandValue = new[]
            {
                Code.Ldc_I4_S, Code.Ldc_I4, Code.Ldc_R4, Code.Ldc_R8, Code.Ldc_I8
            };

            var code = instruction.OpCode.Code;
            if (imeddiateFixedValue.Contains(code))
            {
                return Int32.Parse(code.ToString().Last().ToString());
            }
            else if (imeddiateOperandValue.Contains(code))
            {
                return ((int)instruction.Operand);
            }
            else if (code == Code.Ldc_I4_M1)
            {
                return -1;
            }
            return null;
        }

        private int GetStackPopCount(Instruction instruction)
        {
            StackBehaviour[] pop1Codes = { StackBehaviour.Pop1, StackBehaviour.Popi, StackBehaviour.Popref, };
            StackBehaviour[] pop2Codes =
            {
                StackBehaviour.Pop1_pop1, StackBehaviour.Popi_pop1, StackBehaviour.Popi_popi, StackBehaviour.Popi_popi8, StackBehaviour.Popi_popr4,
                StackBehaviour.Popi_popr8, StackBehaviour.Popref_pop1,  StackBehaviour.Popref_popi
            };
            StackBehaviour[] pop3Codes =
            {
                StackBehaviour.Popi_popi_popi, StackBehaviour.Popref_popi_popi, StackBehaviour.Popref_popi_popi8, StackBehaviour.Popref_popi_popr4,
                StackBehaviour.Popref_popi_popr4, StackBehaviour.Popref_popi_popr8, StackBehaviour.Popref_popi_popref
            };

            if (CodeGroups.CallCodes.Contains(instruction.OpCode.Code))
            {
                return 0;
                //return ((Mono.Cecil.MethodReference) instruction.Operand).Parameters.Count;
            }
            else if (instruction.OpCode.Code == Code.Ret)
            {
                if (Method.ReturnType.FullName == "System.Void")
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else if (Instruction.OpCode.StackBehaviourPop == StackBehaviour.Pop0)
            {
                return 0;
            }
            else if (pop1Codes.Contains(instruction.OpCode.StackBehaviourPop))
            {
                return 1;
            }
            else if (pop2Codes.Contains(instruction.OpCode.StackBehaviourPop))
            {
                return 2;
            }
            else if (pop3Codes.Contains(instruction.OpCode.StackBehaviourPop))
            {
                return 3;
            }
            return 0;
        }

        private int GetStackPushCount(Instruction instruction)
        {
            if (InlineCallModifier.CallOpCodes.Contains(instruction.OpCode.Code))
            {
                return 0;
            }
            if (instruction.OpCode.Code == Code.Ret)
            {
                if (Method.ReturnType.FullName == "System.Void")
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            switch (instruction.OpCode.StackBehaviourPush)
            {
                case StackBehaviour.Push0:
                    return 0;
                case StackBehaviour.Push1_push1:
                    return 2;
                default:
                    return 1;
            }
        }

        public void AddBackDataflowTwoWaySingleIndex(IEnumerable<InstructionWrapper> wrappersToAdd)
        {
            BackDataFlowRelated.AddSingleIndex(wrappersToAdd);
            foreach (var instWrapper in wrappersToAdd)
            {
                instWrapper.ForwardDataFlowRelated.AddSingleIndex(this);
            }
        }

        public void AddForwardTwoWaySingleIndex(IEnumerable<InstructionWrapper> wrappersToAdd)
        {
            BackDataFlowRelated.AddSingleIndex(wrappersToAdd);
            foreach (var instWrapper in wrappersToAdd)
            {
                instWrapper.BackDataFlowRelated.AddSingleIndex(this);
            }
        }

        public int ArgIndex { get; set; }
        public ArgList BackDataFlowRelated
        {
            get
            {
                return _BackDataFlowRelated;
            }
            internal set
            {
                _BackDataFlowRelated = value;
            }
        }
        public List<InstructionWrapper> BackProgramFlow { get; set; } = new List<InstructionWrapper>();
        public ArgList ForwardDataFlowRelated { get; internal set; } = new ArgList();
        public int? ImmediateIntValue { get; private set; }
        public bool Inlined { get; set; } = false;
        public Instruction Instruction { get; set; }
        public int InstructionIndex { get; internal set; }
        public int LocIndex { get; set; }
        public bool MarkForDebugging { get; internal set; }
        public int MemoryReadCount { get; set; }
        public int MemoryStoreCount { get; set; }
        public MethodDefinition Method { get; set; }
        public List<InstructionWrapper> NextPossibleProgramFlow { get; set; } = new List<InstructionWrapper>();
        public int StackPopCount { get; set; }
        public int StackPushCount { get; set; }
        public bool FirstLineInstruction { get; set; } = false;
        public bool WasTreated { get; set; } = false;
    }
}