using System;

namespace ORMLib.exception
{
    [Serializable]
    public class WrongSqlRequestException : Exception
    {
        public WrongSqlRequestException()
        {
        }

        public WrongSqlRequestException(string message) : base(message)
        {
        }
    }
}