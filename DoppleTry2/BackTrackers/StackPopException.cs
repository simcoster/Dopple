using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Dopple.InstructionNodes;

namespace Dopple
{
    [Serializable]
    internal class StackPopException : Exception
    {
        private string v;
        private List<InstructionNode> visitedNodes;

        public StackPopException()
        {
        }

        public StackPopException(string message) : base(message)
        {
        }

        public StackPopException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public StackPopException(string v, List<InstructionNode> visitedNodes)
        {
            this.v = v;
            this.visitedNodes = visitedNodes;
        }

        protected StackPopException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}