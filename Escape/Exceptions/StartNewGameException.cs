using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Escape.Exceptions
{
    [Serializable]
    public class StartNewGameException : FlowControlException
    {
        public StartNewGameException() { }
        public StartNewGameException(string message) : base(message) { }
        public StartNewGameException(string message, Exception inner) : base(message, inner) { }
        protected StartNewGameException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
