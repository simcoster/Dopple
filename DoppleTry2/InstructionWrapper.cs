using System.Collections.Generic;
using System.Linq;
using DoppleTry2.InstructionModifiers;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace DoppleTry2
{
    public class InstructionWrapper
    {
        public Instruction Instruction { get; set; }
        public MethodDefinition Method { get; set; }
        public int StackPushCount { get; set; }
        public bool WasTreated { get; set; } = false;
        public bool HasBackRelated { get; set; } = true;
        public int StackPopCount { get; set; }
        public int MemoryStoreCount { get; set; }
        public int MemoryReadCount { get; set; }
        public List<InstructionWrapper>  NextPossibleProgramFlow { get; set; } = new List<InstructionWrapper>();
        public List<InstructionWrapper> BackProgramFlow { get; set; } = new List<InstructionWrapper>();
        public List<InstructionWrapper> BackDataFlowRelated { get; internal set; } = new List<InstructionWrapper>();
        public List<InstructionWrapper> ForwardDataFlowRelated { get; internal set; } = new List<InstructionWrapper>();
        public int LocIndex { get; set; }
        //TODO : this should be a different thing
        public bool Inlined { get; set; } = false;
        public int StackSum { get; internal set; } = 0;
        public int InstructionIndex { get; internal set; }


        public InstructionWrapper(Instruction instruction , MethodDefinition method)
        {
            Instruction = instruction;
            Method = method;
            StackPushCount = GetStackPushCount(instruction);
            StackPopCount = GetStackPopCount(instruction);
            MemoryReadCount = MemoryProperties.GetMemReadCount(instruction.OpCode.Code);
            MemoryStoreCount = MemoryProperties.GetMemStoreCount(instruction.OpCode.Code);
            LocIndex = LdStLocProperties.GetLocIndex(instruction);
        }

        private int GetStackPushCount(Instruction instruction)
        {
            if (instruction.OpCode.Code == Code.Ret)
            {
                return 1;
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


        private int GetStackPopCount(Instruction instruction)
        {
            StackBehaviour[] pop1Codes = {StackBehaviour.Pop1, StackBehaviour.Popi, StackBehaviour.Popref, };
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

            if (InlineCall.CallOpCodes.Contains(instruction.OpCode.Code))
            {
                return ((Mono.Cecil.MethodReference) instruction.Operand).Parameters.Count;
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
    }

}