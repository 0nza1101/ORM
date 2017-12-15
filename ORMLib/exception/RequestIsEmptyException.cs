using System;

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