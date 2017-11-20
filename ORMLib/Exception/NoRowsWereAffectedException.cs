using System;
using System.Runtime.Serialization;

namespace ORMLib.Database
{
    [Serializable]
    internal class NoRowsWereAffectedException : Exception
    {
        public NoRowsWereAffectedException()
        {
        }

        public NoRowsWereAffectedException(string message) : base(message)
        {
        }

        public NoRowsWereAffectedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoRowsWereAffectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}