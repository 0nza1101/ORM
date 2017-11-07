using System;
using System.Collections.Generic;
using System.Text;

using ORMLib.Constants;

namespace ORMLib.Database
{
    public interface IDatabase
    {
        string connectionString { get; set; }

        void Select(string req);
    }
}
