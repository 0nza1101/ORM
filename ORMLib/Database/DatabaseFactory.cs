using System;
using System.Collections.Generic;
using System.Text;

using ORMLib.Constants;

namespace ORMLib.Database
{
    class DatabaseFactory
    {
        public IDatabase Create(string ip, string port, string dbName, string username, string password, DatabaseType databaseType)
        {
            switch(databaseType)
            {
                case DatabaseType.MSSql:
                    return new MSSql(ip, port, dbName, username, password);
                case DatabaseType.MySql:
                    return new Mysql(ip,dbName, username, password);
                case DatabaseType.PostgreSql:
                    return new MSSql(ip, port, dbName, username, password);
                default:
                    throw new ArgumentException($"DatabaseTypes {databaseType} not supported");
            }
        }
    }
}
