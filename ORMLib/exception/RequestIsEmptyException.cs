using System;
using System.Runtime.Serialization;

namespace ORMLib.exception
{
    [Serializable]
    public class RequestIsEmptyException : Exception
    {
        public RequestIsEmptyException()
        {
        }

        public RequestIsEmptyException(string message) : base(message)
        {
        }
    }
}