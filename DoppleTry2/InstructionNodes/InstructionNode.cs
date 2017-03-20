using System.Collections.Generic;
using System.Linq;
using Dopple.InstructionModifiers;
using Mono.Cecil.Cil;
using Mono.Cecil;
using System;
using Dopple.InstructionWrapperMembers;
using System.Runtime.Serialization;

namespace Dopple.InstructionNodes
{
    [DataContract(IsReference = true), 
        KnownType(typeof(LdImmediateInstNode)) ,KnownType(typeof(LdArgInstructionNode)),
        KnownType(typeof(LdElemInstructionNode)), KnownType(typeof(ArithmaticsNode)),
        KnownType(typeof(RetInstructionNode)), KnownType(typeof(ConditionalJumpNode))]
    public class InstructionNode
    {
        public InstructionNode(Instruction instruction, MethodDefinition method)
        {
            Instruction = instruction;
            Method = method;
            // TODO rework, this must precede StackPopCount for now, yuck
            DataFlowBackRelated = new DataFlowBackArgList(this);
            StackPushCount = GetStackPushCount(instruction);
            StackPopCount = GetStackPopCount(instruction);
            MemoryReadCount = MemoryProperties.GetMemReadCount(instruction.OpCode.Code);
            MemoryStoreCount = MemoryProperties.GetMemStoreCount(instruction.OpCode.Code);
            ProgramFlowBackRoutes = new ProgramFlowBackRoutes(this);
            ProgramFlowBackAffected = new ProgramFlowBackAffectedArgList(this);
            ProgramFlowForwardRoutes = new ProgramFlowForwardRoutes(this);
            DataFlowForwardRelated = new DataFlowForwardArgList(this);
            SingleUnitBackRelated = new SingleUnitBackRelated(this);
            SingleUnitForwardRelated= new SingleUnitForwardRelated(this);
            ProgramFlowForwardAffecting = new ProgramFlowForwardAffectingArgList(this);
            SingleUnitNodes = new List<InstructionNode>();
            MethodNameForSerilization = method.Name;
        }
        
        public ProgramFlowBackRoutes ProgramFlowBackRoutes { get; set; }
        public ProgramFlowForwardRoutes ProgramFlowForwardRoutes { get; set; }
        public DataFlowForwardArgList DataFlowForwardRelated { get; private set; }
        [DataMember]
        public DataFlowBackArgList DataFlowBackRelated { get; protected set; }
        [DataMember]
        public ProgramFlowForwardAffectingArgList ProgramFlowForwardAffecting { get; internal set; }
        public ProgramFlowBackAffectedArgList ProgramFlowBackAffected { get; set; }
        public SingleUnitBackRelated SingleUnitBackRelated { get; set; }
        public SingleUnitForwardRelated SingleUnitForwardRelated { get; internal set; }

        public Instruction Instruction { get; set; }
        [DataMember]
        public int InstructionIndex { get; internal set; }
        public bool MarkForDebugging { get; internal set; }
        public int MemoryReadCount { get; set; }
        public int MemoryStoreCount { get; set; }
        public MethodDefinition Method { get; set; }
        [DataMember]
        public string MethodNameForSerilization { get; set; }
        public bool StackBacktraceDone { get; set; } = false;
        public virtual int StackPopCount
        {
            get
            {
                return _StackPopCount;
            }
            protected set
            {
                _StackPopCount = value;
                DataFlowBackRelated.ResetIndex();
            }
        }
        public int StackPushCount { get; set; }
        public List<Type> DoneBackTracers = new List<Type>();
        public bool ProgramFlowResolveDone { get; set; } = false;
        public Guid MyGuid { get; private set; }
        public List<InstructionNode> SingleUnitNodes { get; private set; }
        public virtual bool DataChangingNode { get; } = false;

        public InliningProperties InliningProperties = new InliningProperties();
        private int _StackPopCount;

        protected int GetStackPopCount(Instruction instruction)
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

            if (Instruction.OpCode.StackBehaviourPop == StackBehaviour.Pop0)
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

        public virtual void MergeInto(InstructionNode nodeToMergeInto, bool keepOriginal)
        {
            if (nodeToMergeInto == this)
            {
                throw new Exception("Can't merge into self");
            }
            foreach (IMergable args in new IMergable[] { DataFlowBackRelated, DataFlowForwardRelated, ProgramFlowBackAffected, ProgramFlowForwardAffecting, ProgramFlowBackRoutes, ProgramFlowForwardRoutes })
            {
                args.MergeInto(nodeToMergeInto, keepOriginal);
            }
        }

        public IEnumerable<InstructionNode> GetDataOriginNodes()
        {
            var justThis = new[] { this };
            if (DataFlowBackRelated.Count ==0)
            {
                return justThis;
            }
            var thisAsDataTrasferingNode = this as IDataTransferingNode;
            if (thisAsDataTrasferingNode == null)
            {
                return justThis;
            }
            if (DataFlowBackRelated.Any(x => x.ArgIndex == thisAsDataTrasferingNode.DataFlowDataProdivderIndex) == false)
            {
                return justThis;
            }
            return DataFlowBackRelated.Where(x => x.ArgIndex == thisAsDataTrasferingNode.DataFlowDataProdivderIndex)
                                      .SelectMany(x => x.Argument.GetDataOriginNodes());
        }

        internal void SelfRemove()
        {
            DataFlowForwardRelated.RemoveAllTwoWay();
            DataFlowBackRelated.RemoveAllTwoWay();
            ProgramFlowBackAffected.RemoveAllTwoWay();
            ProgramFlowForwardAffecting.RemoveAllTwoWay();
            foreach (var forwardRoute in ProgramFlowForwardRoutes)
            {
                forwardRoute.ProgramFlowBackRoutes.AddTwoWay(ProgramFlowBackRoutes);
            }
            ProgramFlowBackRoutes.RemoveAllTwoWay();
            ProgramFlowForwardRoutes.RemoveAllTwoWay();
        }


        internal virtual void SelfRemoveAndStitch()
        {
            foreach (var forwardInst in DataFlowForwardRelated.ToArray())
            {
                int index = forwardInst.ArgIndex;
                forwardInst.MirrorArg.ContainingList.AddTwoWay(DataFlowBackRelated.Select(x => x.Argument).ToList(), index);
                forwardInst.ContainingList.RemoveTwoWay(forwardInst);
            }
            foreach (var forwardPath in ProgramFlowForwardRoutes.ToArray())
            {
                forwardPath.ProgramFlowBackRoutes.AddTwoWay(ProgramFlowBackRoutes.ToList());
                ProgramFlowForwardRoutes.RemoveTwoWay(forwardPath);
            }
            foreach (CoupledIndexedArgList args in new CoupledIndexedArgList[]{ DataFlowBackRelated, ProgramFlowBackAffected, ProgramFlowForwardAffecting})
            {
                args.RemoveAllTwoWay();
            }
            foreach (CoupledList related in new CoupledList[] { ProgramFlowBackRoutes, SingleUnitBackRelated, SingleUnitForwardRelated})
            {
                related.RemoveAllTwoWay();
            }
        }
    }
}