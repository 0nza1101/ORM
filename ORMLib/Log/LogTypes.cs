using System;
using System.Collections.Generic;
using System.Text;

namespace ORMLib.Log
{
    [Flags]
    enum LogTypes
    {
        None = 0x0,
        Info = 0x1,
        Warning = 0x2,
        Error = 0x4
    }
}
