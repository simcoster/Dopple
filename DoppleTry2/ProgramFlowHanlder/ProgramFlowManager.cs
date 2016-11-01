using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoppleTry2.ProgramFlowHanlder
{
    class ProgramFlowManager
    {
        private readonly IEnumerable<ProgramFlowHandler> _flowHandlers;
        public ProgramFlowManager()
        {
            _flowHandlers = 
               GetType()
                   .Assembly.GetTypes()
                   .Where(x => typeof(ProgramFlowHandler).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                   .Select(x => Activator.CreateInstance(x))
                   .Cast<ProgramFlowHandler>();
        }

        public void AddFlowConnections(List<InstructionWrapper> instructionsWrappers)
        {
            foreach (var instWrapper in instructionsWrappers)
            {
                var flowHandlers = _flowHandlers.Where(x => x.HandledCodes.Contains(instWrapper.Instruction.OpCode.Code));
                foreach (var flowHandler in flowHandlers)
                {
                    flowHandler.SetForwardExecutionFlowInsts(instWrapper, instructionsWrappers);
                }
            }
            foreach (var instWrapper in instructionsWrappers)
            {
                instWrapper.InstructionIndex = instructionsWrappers.IndexOf(instWrapper);
            }
        }
    }
}
