using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Escape.Exceptions
{
    [Serializable]
    public class QuitGameException : FlowControlException
    {
        public QuitGameException() { }
        public QuitGameException(string message) : base(message) { }
        public QuitGameException(string message, Exception inner) : base(message, inner) { }
        protected QuitGameException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
