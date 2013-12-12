using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Escape.Exceptions
{
    // Normally one would probably not use exceptions for this but it's convenient in this case.
    // Using exceptions to unroll the stack to the highest level means you can restart without building up frames (allocations on the stack).
    // Note that this runs very slowly while debugging, as getting the call stack is a constly function and throwing notifies the debugger.

    // There is most likely a better solution involving different input handling,
    // and possibly coroutines (to keep the state stack but make it pauseable and resumable).
    // I may try to come up with something after writing a more in-depth explanation, right now it's just speculation.
    [Serializable]
    public class FlowControlException : EscapeException
    {
        public FlowControlException() { }
        public FlowControlException(string message) : base(message) { }
        public FlowControlException(string message, Exception inner) : base(message, inner) { }
        protected FlowControlException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
