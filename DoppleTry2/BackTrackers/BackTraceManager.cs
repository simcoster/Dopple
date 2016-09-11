using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;

namespace DoppleTry2.BackTrackers
{
    public class BackTraceManager
    {
        private readonly List<InstructionWrapper> _instructionsWrappers;
        private readonly Dictionary<Code, BackTracer> _backtracersCodes = new Dictionary<Code, BackTracer>();

        public BackTraceManager(List<InstructionWrapper> instructionsWrappers)
        {
            List<BackTracer> backTracers = new List<BackTracer>();
            _instructionsWrappers = instructionsWrappers;

            backTracers.Add(new LdLocBackTracer(_instructionsWrappers));

            var allCodes = typeof(OpCodes).GetFields().Select(x => x.GetValue(null)).Cast<OpCode>();

            List<Code> deltCodes = backTracers.SelectMany(x => x.HandlesCodes).ToList();

            var undeltCodes = typeof(OpCodes).GetFields().Select(x => x.GetValue(null)).Cast<OpCode>().Select(y => y.Code)
                .Except(backTracers.SelectMany(x=> x.HandlesCodes))
                .Except(Program.UnaffectingCodes).ToArray();

            // run pop backtracer first, since it modiefies the rest

            // then run dup backtracer
        }

        public void Run()
        {
            bool done = false;
            while (!done)
            {
                for (int i = _instructionsWrappers.Count; i >= 0; i--)
                {
                    if (i == 0)
                    {
                        done = true;
                        break;
                    }
                    var currInstruction = _instructionsWrappers[i];
                    if (currInstruction.WasTreated == false && currInstruction.HasBackRelated)
                    {
                        BackTraceInst(currInstruction);
                        break;
                    }
                   
                }
            }
        }

        private void BackTraceInst(InstructionWrapper instruction)
        {
            if (instruction.HasBackRelated == false)
            {
                return;
            }
            _backtracersCodes[instruction.Instruction.OpCode.Code].AddBackDataflowConnections(instruction);
        }
    }
}