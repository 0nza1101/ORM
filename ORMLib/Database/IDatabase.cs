using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using ORMLib.Constants;

namespace ORMLib.Database
{
    public interface IDatabase
    {
        string connectionString { get; set; }

        List<T> Select<T>(string req);
    }
}
