using DoppleTry2.BackTrackers;
using DoppleTry2.ProgramFlowHanlder;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.InstructionModifiers
{
    class RecursionStArgModifier : IPostBackTraceModifier
    {
        public void Modify(List<InstructionWrapper> instructionWrappers)
        {
            for (int i = 0; i < instructionWrappers.Count; i++)
            {
                var instWrapper = instructionWrappers[i];
                if (CodeGroups.CallCodes.Contains(instWrapper.Instruction.OpCode.Code) &&
                            instWrapper.Instruction.Operand is MethodDefinition &&
                            instWrapper.Inlined == false &&
                            ((MethodDefinition)instWrapper.Instruction.Operand).Name == instWrapper.Method.Name)
                {
                    var flowHandler = new SimpleProgramFlowHandler(instructionWrappers);
                    //var prevConnectedInstructions = instWrapper.BackProgramFlow.SelectMany(x => flowHandler.GetAllPreviousConnected(x)).ToList();
                    var addedInsts = StArgAdder.InsertHelperSTargs(instructionWrappers, instWrapper, instWrapper.Method);
                    instWrapper.Inlined = true;
                    LdArgBacktracer ldArgBackTracer = new LdArgBacktracer(null);
                    var ldArgInstructions = instructionWrappers
                                           .Where(x => CodeGroups.LdArgCodes.Contains(x.Instruction.OpCode.Code))
                                           .Where(x => x.Method == instWrapper.Method)
                                           .OrderBy(x => x.ArgIndex)
                                           .GroupBy(x => x.ArgIndex);
                    foreach (var ldArgGroup in ldArgInstructions)
                    {
                        foreach (var ldArgInst in ldArgGroup)
                        {
                            if (ldArgInst.BackDataFlowRelated.ArgumentList.Count == 0)
                            {
                                ldArgInst.FirstLineInstruction = true;
                            }
                            var addedGroup = addedInsts.Where(x => x.ArgIndex == ldArgInst.ArgIndex);
                            ldArgInst.BackDataFlowRelated.AddSingleIndex(addedGroup);
                            foreach (var addedInst in addedGroup)
                            {
                                addedInst.ForwardDataFlowRelated.AddSingleIndex(ldArgInst);
                            }
                        }
                    }
                }

            }
        }
    }
}
