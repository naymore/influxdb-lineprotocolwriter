using System;
using System.Runtime.Serialization;

namespace Rs.InfluxDb.LineProtocolWriter.Exceptions
{
    [Serializable]
    internal class LineProtocolException : Exception
    {
        public LineProtocolException()
        {
        }

        public LineProtocolException(string message) : base(message)
        {
        }

        public LineProtocolException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LineProtocolException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}