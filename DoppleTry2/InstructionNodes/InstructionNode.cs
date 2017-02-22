using System.Collections.Generic;
using System.Linq;
using Dopple.InstructionModifiers;
using Mono.Cecil.Cil;
using Mono.Cecil;
using System;
using Dopple.InstructionWrapperMembers;

namespace Dopple.InstructionNodes
{
    public class InstructionNode
    {

        public InstructionNode(Instruction instruction, MethodDefinition method)
        {
            Instruction = instruction;
            Method = method;
            StackPushCount = GetStackPushCount(instruction);
            StackPopCount = GetStackPopCount(instruction);
            MemoryReadCount = MemoryProperties.GetMemReadCount(instruction.OpCode.Code);
            MemoryStoreCount = MemoryProperties.GetMemStoreCount(instruction.OpCode.Code);
            DataFlowBackRelated = new DataFlowBackArgList(this);
            ProgramFlowBackRoutes = new ProgramFlowBackRoutes(this);
            ProgramFlowBackAffected = new ProgramFlowBackAffectedArgList(this);
            ProgramFlowForwardRoutes = new ProgramFlowForwardRoutes(this);
            DataFlowForwardRelated = new DataFlowForwardArgList(this);
            SingleUnitBackRelated = new SingleUnitBackRelated(this);
            SingleUnitForwardRelated= new SingleUnitForwardRelated(this);
            ProgramFlowForwardAffecting = new ProgramFlowForwardAffectingArgList(this);
            SingleUnitNodes = new List<InstructionNode>();
            MyGuid = Guid.NewGuid();
        }

        public ProgramFlowBackRoutes ProgramFlowBackRoutes { get; set; }
        public ProgramFlowForwardRoutes ProgramFlowForwardRoutes { get; set; }
        public DataFlowForwardArgList DataFlowForwardRelated { get; private set; }
        virtual public DataFlowBackArgList DataFlowBackRelated { get; private set; }
        public ProgramFlowForwardAffectingArgList ProgramFlowForwardAffecting { get; internal set; }
        public ProgramFlowBackAffectedArgList ProgramFlowBackAffected { get; set; }
        public SingleUnitBackRelated SingleUnitBackRelated { get; set; }
        public SingleUnitForwardRelated SingleUnitForwardRelated { get; internal set; }

        public Instruction Instruction { get; set; }
        public int InstructionIndex { get; internal set; }
        public bool MarkForDebugging { get; internal set; }
        public int MemoryReadCount { get; set; }
        public int MemoryStoreCount { get; set; }
        public MethodDefinition Method { get; set; }
        public virtual int StackPopCount { get; set; }
        public int StackPushCount { get; set; }
        public List<Type> DoneBackTracers = new List<Type>();
        public bool ProgramFlowResolveDone { get; set; } = false;
        public Guid MyGuid { get; private set; }
        public List<InstructionNode> SingleUnitNodes { get; private set; }

        public InliningProperties InliningProperties = new InliningProperties();

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

        public void MergeInto(InstructionNode nodeToMergeInto)
        {
            foreach (IMergable args in new IMergable[] { DataFlowBackRelated, DataFlowForwardRelated, ProgramFlowBackAffected, ProgramFlowForwardAffecting, ProgramFlowBackRoutes, ProgramFlowForwardRoutes})
            {
                args.MergeInto(nodeToMergeInto);
            }
        }


        internal virtual void SelfRemove()
        {
            foreach (var forwardInst in DataFlowForwardRelated.ToArray())
            {
                int index = forwardInst.ArgIndex;
                forwardInst.MirrorArg.ContainingList.AddTwoWay(DataFlowBackRelated.Select(x => x.Argument), index);
                forwardInst.ContainingList.RemoveTwoWay(forwardInst);
            }
            foreach (var forwardPath in ProgramFlowForwardRoutes.ToArray())
            {
                forwardPath.ProgramFlowBackRoutes.AddTwoWay(ProgramFlowBackRoutes);
                ProgramFlowForwardRoutes.RemoveTwoWay(forwardPath);
            }
            foreach (ArgList args in new ArgList[]{ DataFlowBackRelated, ProgramFlowBackAffected, ProgramFlowForwardAffecting})
            {
                args.RemoveAllTwoWay();
            }
            foreach (RelatedList related in new RelatedList[] { ProgramFlowBackRoutes, SingleUnitBackRelated, SingleUnitForwardRelated})
            {
                related.RemoveAllTwoWay();
            }
        }
    }
}