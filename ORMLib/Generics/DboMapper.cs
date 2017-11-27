using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;

using ORMLib.Constants;
using ORMLib.Database;
using System.Data.Common;
using ORMLib.exception;

namespace ORMLib.Generics
{
    public class DboMapper
    {
        private IDatabase m_database;

        public IDatabase database
        {
            get { return m_database; }
            private set { m_database = value; }
        }

        /// <summary>
        ///     Creates a DatabaseFactory object that will allow us to call DatabaseFactory.Create() to create our database given the parameters in entry and given the type of database
        ///     Type of Database is required because it is this object that will create the database (MSSql, MySql, PostgreSQL)
        /// </summary>
        /// <param name="ip">The IP Address of the database. It generally is localhost</param>
        /// <param name="port">Optional Value, needed for MSSql</param>
        /// <param name="dbName">Name of your database</param>
        /// <param name="username">Username to connect to the database</param>
        /// <param name="password">Password to connect to the database</param>
        /// <param name="databaseType">Object that defines the type of database that we want to create</param>
        public DboMapper(string ip, string port, string dbName, string username, string password, DatabaseType databaseType)
        {
            //Create the correct Database
            DatabaseFactory databaseFactory = new DatabaseFactory();
            this.database = databaseFactory.Create(ip, port, dbName, username, password, databaseType);
        }

        /// <summary>
        ///     Checks if the word 'SELECT' is in the first position in the parameter 'request'. If not, it returns null.
        ///     And then calls the IDatabase.Select<T>() function and gives him the request as a parameter
        /// </summary>
        /// <typeparam name="T">Generic object </typeparam>
        /// <param name="request">Contains the SQL request</param>
        /// <returns>A list that contains the result of the SQL Statement</returns>
        public List<T> List<T>(string request)
        { 
            //Check if request contain SELECT words at index 0
            if(request.IndexOf("SELECT", StringComparison.Ordinal) != 0){
                throw new WrongSqlRequestException("Your SQL Statement is not a SELECT Statement. The function List<T> can only be used with a SELECT statement");
            }
            
            return database.Select<T>(request);
        }

        /// <summary>
        ///     Calls the function IDatabase.Execute<T>() and gives him the SQL Statement and the list of the parameters
        /// </summary>
        /// <typeparam name = "T"> Generic object </typeparam>
        /// <param name="request">Contains the SQL request</param>
        /// <param name="parameters">Contains the values of the parameters of the SQL Statement</param>
        /// <returns>A list that contains the result of the SQL Statement</returns>
        public List<T> Execute<T>(string request, List<DbParameter> parameters)
        {
            if (request.Equals(String.Empty) || request.Equals(null))
            {
                throw new RequestIsEmptyException("You have not provided a SQL statement");
            }  
            return database.Execute<T>(request, parameters);
        }

	}
}
