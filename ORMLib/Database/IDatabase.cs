using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using ORMLib.Constants;
using System.Data.Common;

namespace ORMLib.Database
{
    public interface IDatabase
    {
        string connectionString { get; set; }

        DbConnection dbConnection { get; set; }

        List<T> Select<T>(string req);

        List<T> Execute<T>(string req, List<DbParameter> listOfParameters);
    }
}
