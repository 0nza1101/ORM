using System;
using System.Collections.Generic;
using System.Text;

namespace ORMLib.Log
{
    public interface ILog
    {
        void Message(Enum logTypes, string message);
    }
}
