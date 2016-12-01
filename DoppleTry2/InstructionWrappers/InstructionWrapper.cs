using System.Collections.Generic;
using System.Linq;
using DoppleTry2.InstructionModifiers;
using Mono.Cecil.Cil;
using Mono.Cecil;
using System;

namespace DoppleTry2.InstructionWrappers
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
            BackDataFlowRelated.AddWithNewIndex(wrappersToAdd);
            foreach (var instWrapper in wrappersToAdd)
            {
                instWrapper.ForwardDataFlowRelated.AddWithNewIndex(this);
            }
        }

        public void AddForwardTwoWaySingleIndex(IEnumerable<InstructionWrapper> wrappersToAdd)
        {
            BackDataFlowRelated.AddWithNewIndex(wrappersToAdd);
            foreach (var instWrapper in wrappersToAdd)
            {
                instWrapper.BackDataFlowRelated.AddWithNewIndex(this);
            }
        }

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
        ArgList forwardDataFlowRelated = new ArgList();
        public ArgList ForwardDataFlowRelated
        {
            get
            {
                return forwardDataFlowRelated;
            }
            internal set
            {
                forwardDataFlowRelated = value;
            }
        }
        public Instruction Instruction { get; set; }
        public int InstructionIndex { get; internal set; }
        public bool MarkForDebugging { get; internal set; }
        public int MemoryReadCount { get; set; }
        public int MemoryStoreCount { get; set; }
        public MethodDefinition Method { get; set; }
        public List<InstructionWrapper> ForwardProgramFlow { get; set; } = new List<InstructionWrapper>();
        public int StackPopCount { get; set; }
        public int StackPushCount { get; set; }
        public bool FirstLineInstruction { get; set; } = false;
        public List<Type> DoneBackTracers = new List<Type>();
        public bool ProgramFlowResolveDone { get; set; } = false;
        public InliningProperties InliningProperties = new InliningProperties();
    }

   
}