using System;
using System.Collections.Generic;
using System.Text;

using ORMLib.Constants;

namespace ORMLib.Database
{
    class DatabaseFactory
    {
        public IDatabase Create(string ip, string dbName, string username, string password, DatabaseType databaseType)
        {
            try
            {
                switch(databaseType)
                {
                    case DatabaseType.MSSql:
                        return new MSSql(ip, dbName, username, password);
                    case DatabaseType.MySql:
                        return new MSSql(ip, dbName, username, password);
                    case DatabaseType.PostgreSql:
                         return new MSSql(ip, dbName, username, password);
                    default:
                        throw new ArgumentException($"DatabaseTypes {databaseType} not supported");
                }
            }
            catch (NotImplementedException e)
            {
                Console.WriteLine("Erreur : ", e.Message);
            }
            return new MSSql(ip, dbName, username, password);
        }

    }
}
