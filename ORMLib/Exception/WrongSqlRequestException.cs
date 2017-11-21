using System;
using System.Runtime.Serialization;

namespace ORMLib.Database
{
    [Serializable]
    internal class WrongSqlRequestException : Exception
    {
        public WrongSqlRequestException()
        {
        }

        public WrongSqlRequestException(string message) : base(message)
        {
        }

        public WrongSqlRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WrongSqlRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}