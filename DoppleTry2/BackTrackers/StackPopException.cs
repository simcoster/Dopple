using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Dopple.InstructionNodes;

namespace Dopple
{
    [Serializable]
    public class StackPopException : Exception
    {
        private string v;
        public List<InstructionNode> problematicRoute;

        public StackPopException(string v, List<InstructionNode> visitedNodes)
        {
            this.v = v;
            this.problematicRoute = visitedNodes;
        }

        protected StackPopException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}