using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Escape.Exceptions
{
    [Serializable]
    public class EscapeException : Exception
    {
        public EscapeException() { }
        public EscapeException(string message) : base(message) { }
        public EscapeException(string message, Exception inner) : base(message, inner) { }
        protected EscapeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
