using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Data.Common;
using ORMLib.exception;
using Npgsql;


namespace ORMLib.Database
{
    public class PostgreSql : IDatabase
    {
        private DbConnection _dbConnection;
        private string m_connectionString;


        public string connectionString
        {
            get { return m_connectionString; }
            set { m_connectionString = value; }
        }

        public DbConnection dbConnection
        {
            get
            {
                return this._dbConnection;
            }
            set
            {
                this._dbConnection = value;
            }
        }

        /// <summary>
        ///     Setting the connectionString values.
        /// </summary>
        /// <param name="ip">The IP Address of the database, which is usually 'localhost'</param>
        /// <param name="dbName">Name given to the database</param>
        /// <param name="username">Username to connect to the database</param>
        /// <param name="password">Password set to connect to the postgre database. Did not set one here.</param>
        public PostgreSql(string ip, string dbName, string username, string password)
        {
            // Specify connection options and open an connection
            connectionString = String.Format("Server={0};User Id={2}; Password={3};Database={1};",
                                             ip, dbName,username, password);
        }

        /// <summary>
        ///     SELECT statement.
        ///     
        ///     It specifies connection options and opens it.
        ///     It takes a DbConnection and creates a DbCommand to retrieve data by executing a DbDataReader.
        ///     Then we return this list.
        ///     
        ///     "DbConnection : gets or sets the DbConnection used by this DbCommand"
        ///     "DbCommand : Represents an SQL statement or stored procedure to execute against a data source, it provides a base class for database-specific classes that represent commands."
        ///     "The DbDataReader reads a forward-only stream of rows from a data source."
        ///     
        ///     As for the try-catch statement, when an exception is thrown, it will look for the catch statement that handles this exception. 
        ///     It will display an unhandled exception message if the catch block is not found.
        /// </summary>
        /// <typeparam name="T">A generic object, can be int, can typeof Contacts</typeparam>
        /// <param name="req">Contains the SQL Statement</param>
        /// <returns>A list that contains the result of the SQL SELECT statement</returns>
        public List<T> Select<T>(string req)
        {
            using (dbConnection = new NpgsqlConnection())
            {
                List<T> list = new List<T>();
                T obj = default(T);

                dbConnection.ConnectionString = connectionString;
                dbConnection.Open();
                DbCommand dbCommand = dbConnection.CreateCommand();
                dbCommand.CommandText = req;
                dbCommand.CommandType = CommandType.Text;
                dbCommand.Connection = dbConnection;
                DbDataReader reader;
                try
                {
                    reader = dbCommand.ExecuteReader();
                }
                catch (Exception e)
                {
                    throw new DatabaseException($"Problème lors de l'exécution de la requête SQL : {e.Message}");
                }

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        obj = Activator.CreateInstance<T>();
                        foreach (PropertyInfo prop in obj.GetType().GetProperties())
                        {
                            if (!reader[prop.Name].Equals(null))
                            {
                                prop.SetValue(obj, reader[prop.Name], null);
                            }
                        }
                        list.Add(obj);
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                // Always call Close when done reading.
                // Dispose releases all the ressources used by dbConnection
                reader.Close();
                dbConnection.Close();
                dbConnection.Dispose();
                return list;
            }
        }

        /// <summary>
        ///     EXECUTE statement here.
        ///     It is supposed to execute the chosen statement implying the parameters values set in listOfParameters, and opens the connection.
        ///     If the SELECT statement is chosen, it will call the ExecuteReader(). It checks the returned row and creates an object with the results of the selected statement.
        ///     As for the others, based on those INSERT, UPDATE, and DELETE requests, it will call the ExecuteNonQuery():
        ///     Whenever you want to execute an SQL statement that shouldn't return a value or a record set, the ExecuteNonQuery should be used. 
        ///     It returns the number of rows affected by the statement. If there is none, an exception will be thrown.
        /// </summary>
        /// <typeparam name="T">A generic object, can be int, can typeof Contacts</typeparam>
        /// <param name="req">Contains the SQL statement</param>
        /// <param name="listOfParameters">Contains the value of the parameters in the SQL Statement</param>
        /// <returns>It either returns a list that contains the result of a SELECT Sql Statement, or it returns an empty list</returns>
        public List<T> Execute<T>(string req, List<DbParameter> listOfParameters)
        {
            List<T> list = new List<T>();
            using (dbConnection = new NpgsqlConnection())
            {
                dbConnection.ConnectionString = connectionString;
                DbCommand dbCommand = new NpgsqlCommand();
                dbCommand.CommandText = req;
                dbCommand.CommandType = CommandType.Text;
                dbCommand.Connection = dbConnection;
                foreach (DbParameter param in listOfParameters)
                {
                    DbParameter npgSqlParameter = dbCommand.CreateParameter();
                    npgSqlParameter.ParameterName = param.ParameterName;
                    npgSqlParameter.Value = param.Value;
                    dbCommand.Parameters.Add(npgSqlParameter);
                }
                dbConnection.Open();
                var i = 0;
                if (req.IndexOf("SELECT", StringComparison.Ordinal) == 0)
                {
                    DbDataReader dbDataReader;
                    try
                    {
                        dbDataReader = dbCommand.ExecuteReader();
                    }
                    catch (Exception e)
                    {
                        throw new DatabaseException($"An error has occured. Reason : {e.Message}");
                    }

                    T obj = default(T);
                    if (dbDataReader.HasRows)
                    {
                        while (dbDataReader.Read())
                        {
                            obj = Activator.CreateInstance<T>();
                            foreach (PropertyInfo prop in obj.GetType().GetProperties())
                            {
                                if (!dbDataReader[prop.Name].Equals(null))
                                {
                                    prop.SetValue(obj, dbDataReader[prop.Name]);
                                }
                            }
                            list.Add(obj);
                        }
                        dbDataReader.Close();
                    }
                    else
                    {
                        Console.WriteLine("No rows were found.");
                    }
                }
                else if (req.IndexOf("INSERT", StringComparison.Ordinal) == 0
                    || req.IndexOf("UPDATE", StringComparison.Ordinal) == 0
                    || req.IndexOf("DELETE", StringComparison.Ordinal) == 0)
                {
                    try
                    {
                        i = dbCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        throw new DatabaseException($"An error has occured. Reason : {e.Message}");
                    }
                }
                else
                {
                    throw new WrongSqlRequestException("Your SQL statement is not a SELECT, INSERT, UPDATE or DELETE statement");
                }

                // Closes and releases the ressources used by the component.
                dbConnection.Close();
                dbConnection.Dispose();
                return list;
            }
        }
    }
}

